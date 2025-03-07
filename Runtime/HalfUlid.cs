using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace SatorImaging.HUlid
{
    // TODO: make class non-static or struct for use in multi-threaded apps.
    // TODO v3: to make everything simplified, use elapsed milliseconds from origin DateTime instead of transforming value of year, month, etc.
    //          43bits can represent about 279 years from origin (v2: 127 years)
    //          * as like v2 update, always add 1 year offset to every HalfUlid creation date to make untyped long value identifiable
    //          --> 1 year in msecs: 31_536_000_000
    //          --> shift << 21bits: 66_135_785_472_000_000
    //          or, always set most-significant bit to make it extreme number.
    //          42 bits still be able to represent about 139 years in milliseconds.
    //          --> but sort order is broken (newer-to-older in creation time)
    //          --> use most significant bit to determine overflow?
    //          --> DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
    //          * GUID v7 uses 48 bits room for elapsed milliseconds from UNIX epoch millis to embed creation time
    //            when 64 bit long, remaining 16 bits room can store 65,536 unique number, 15 bits stores 32,768 if most-significant bit is reserved
    //          * simply taking upper 64bits from GUID v7 seems good but Unity doesn't support it.
    /// <summary>
    /// Half-ULID (HUlid) is a 64-bit (long) shrinked version of ULID.
    /// Max 2,097,152 of IDs can be created for every milliseconds.
    /// </summary>
    public static class HalfUlid
    {
        public const int ID_MAX = 2097151;  // 21-bit
        public const int YEAR_MAX = 127;    // 7-bit
        public const int YEAR_USE_CURRENT = -1;

        public const int DEFAULT_YEAR_ORIGIN = 2023 - 1;  // v2 update: -1 to make year offset starts from 1
        public const int DEFAULT_START_VALUE = -1;

        [DescriptionAttribute("v2 update: this value is `CurrentOriginYear - 1`, not actual origin year.")]
        static int _currentOriginYear = DEFAULT_YEAR_ORIGIN;
        static long _currentValue = DEFAULT_START_VALUE;
        static long _timeBits = GetTimeBits(DateTime.UtcNow);
        static bool _isRandomized = false;

        public const int RANDOM_ID_MAX = 8191;  // 13-bit
        public const int RANDOM_ID_SEQ_BIT_LENGTH = 13;
        [Obsolete("Use RANDOM_ID_MAX instead")]
        public const long RANDOM_ID_BITMASK = 0b_0001_1111_1111_1111L;
        readonly static RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        readonly static byte[] _randomBytes = new byte[1];

        // NOTE: long is easier than ulong to use in C#. many of parameters in C# are long, not ulong.
        //       of course Unity follows that. Unity uses long in many places even if it is treated
        //       as ulong in native code, for example, object path id.
        //       when change HUlid value to ulong, many of casting is required in tons of C# codes.
        //       so HUlid value should be long for usability but when comparing with MinValue, values
        //       must be treated as ulong.
        //       defining MinValue as ulong will shows warning and remind that. do not mind consistency.
        public const ulong MinValue = 1UL << 57;

        // properties
        public static int CurrentOriginYear => _currentOriginYear + 1;


        /// <summary>
        /// Initialize creation time used for subsequent <see cref="Next(int)"/> or <see cref="Random()"/> calls.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init() => Init(DEFAULT_START_VALUE, YEAR_USE_CURRENT);

        /// <inheritdoc cref="Init()"/>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Init(int startValue = DEFAULT_START_VALUE, int originYear = YEAR_USE_CURRENT)
        {
            _isRandomized = false;

            if (startValue < DEFAULT_START_VALUE)
                startValue = DEFAULT_START_VALUE;

            _currentValue = startValue;

            if (originYear > YEAR_USE_CURRENT)
            {
                originYear -= 1;  // v2 stores year offset in range 1-127
                var yearOffset = DateTime.UtcNow.Year - originYear;
                if (yearOffset <= 0 || yearOffset > YEAR_MAX)
                    throw new ArgumentOutOfRangeException(nameof(originYear));

                _currentOriginYear = originYear;
            }

            SetCreationTime(DateTime.UtcNow);
        }


        ///<param name="utcTime">Non-UTC time is automatically converted to UTC time.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long GetTimeBits(DateTime utcTime)
        {
            if (utcTime.Kind != DateTimeKind.Utc)
                utcTime = utcTime.ToUniversalTime();

            //overflow??
            var yearOffset = utcTime.Year - _currentOriginYear;
            if (yearOffset <= 0 || yearOffset > YEAR_MAX)
                throw new ArgumentOutOfRangeException(nameof(utcTime), "yearOffset overflows");

            return ((long)yearOffset << 57)
                 | ((long)utcTime.Month << 53)
                 | ((long)utcTime.Day << 48)
                 | ((long)utcTime.Hour << 43)
                 | ((long)utcTime.Minute << 37)
                 | ((long)utcTime.Second << 31)
                 | ((long)utcTime.Millisecond << 21)
                 ;
        }

        ///<param name="utcTime">Non-UTC time is automatically converted to UTC time.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCreationTime(DateTime utcTime)
        {
            _timeBits = GetTimeBits(utcTime);
        }


        ///<summary>Generate sequential Half-ULID value.</summary>
        ///<remarks>Init() is automatically called when maximum id for current creation time is reached.</remarks>
        public static long Next(int offset = 1)
        {
            if (offset <= 0)
                offset = 1;

            _currentValue += offset;

            if (_currentValue > ID_MAX || (_isRandomized && _currentValue > RANDOM_ID_MAX))
                Init(startValue: 0);

            return _timeBits | _currentValue;
        }

        ///<summary>Generate Half-ULID using random number and sequential value.</summary>
        ///<remarks>Init() is automatically called when maximum id for current creation time is reached.</remarks>
        public static long Random()
        {
            _isRandomized = true;
            if (_currentValue >= RANDOM_ID_MAX)
                Init();

            _currentValue++;
            _rng.GetNonZeroBytes(_randomBytes);
            return _timeBits | ((long)_randomBytes[0] << RANDOM_ID_SEQ_BIT_LENGTH) | _currentValue;
        }


        /// <summary>
        /// Set creation date and generate new Half-ULID. (shorthand for <c>Init(); Next/Random();</c>)
        /// </summary>
        public static long New(bool randomOrSequential)
        {
            Init();
            return randomOrSequential ? Random() : Next();
        }


        /*  Get Methods  ================================================================ */

        [Obsolete("Planned to remove in future version.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<returns>Value part of Half-ULID.</returns>
        public static int ToHUlidValue(this long val) => GetValue(val);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<returns>Value part of Half-ULID.</returns>
        public static int GetValue(long val)
        {
            return unchecked((int)(val & ID_MAX));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<returns>Value part of Half-ULID generated using `Random()`.</returns>
        public static int GetValueWithoutRandomBits(long val)
        {
            return unchecked((int)(val & RANDOM_ID_MAX));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<returns>Random part of Half-ULID generated using `Random()`.</returns>
        public static int GetRandomBits(long val)
        {
            return unchecked((int)(val >> RANDOM_ID_SEQ_BIT_LENGTH) & 0b_1111_1111);
        }


        /*  TryGet Methods  ================================================================ */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetValue(long hulid, out int value)
        {
            value = GetValue(hulid);
            return (ulong)hulid >= MinValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetValueWithoutRandomBits(long hulid, out int value)
        {
            value = GetValueWithoutRandomBits(hulid);
            return (ulong)hulid >= MinValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetRandomBits(long hulid, out int value)
        {
            value = GetRandomBits(hulid);
            return (ulong)hulid >= MinValue;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetDateTime(long val, out DateTime utcDateTime, int originYear = YEAR_USE_CURRENT)
        {
            utcDateTime = GetDateTime(val, originYear);
            return utcDateTime != DateTime.MinValue;
        }


        /*  DateTime  ================================================================ */

        [Obsolete("Planned to remove in future version.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<returns>DateTime.MinValue when error.</returns>
        public static DateTime ToHUlidDateTime(this long val, int originYear = YEAR_USE_CURRENT)
            => GetDateTime(val, originYear);

        ///<returns>DateTime.MinValue when error.</returns>
        public static DateTime GetDateTime(long val, int originYear = YEAR_USE_CURRENT)
        {
            if ((ulong)val < MinValue)
                return DateTime.MinValue;

            if (originYear <= YEAR_USE_CURRENT)
                originYear = _currentOriginYear;

            try
            {
                // NOTE: bit operator `>>>` cannot be used in C# 9.0
                //       need to do sign-bit aware operation.
                var year = unchecked((int)((ulong)val >> 57)) + originYear;
                //if (year < 0)
                //{
                //    year &= 0b_0111_1111L;
                //}
                //year += originYear;

                unchecked
                {
                    return new DateTime(
                        year,
                        (int)( /**/ (val & (0b_1111L << 53)) >> 53         /**/ ),
                        (int)( /**/ (val & (0b_1111_1L << 48)) >> 48       /**/ ),
                        (int)( /**/ (val & (0b_1111_1L << 43)) >> 43       /**/ ),
                        (int)( /**/ (val & (0b_1111_11L << 37)) >> 37      /**/ ),
                        (int)( /**/ (val & (0b_1111_11L << 31)) >> 31      /**/ ),
                        (int)( /**/ (val & (0b_1111_1111_11L << 21)) >> 21 /**/ ),
                        DateTimeKind.Utc
                        );
                }
            }
            catch
            {
            }

            return DateTime.MinValue;
        }

    }
}
