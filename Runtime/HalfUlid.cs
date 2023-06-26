using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SatorImaging.HUlid
{
    public static class HalfUlid
    {
        public const int ID_MAX = 2097151;  // 21-bit
        public const int YEAR_MAX = 127;    // 7-bit
        public const int YEAR_USE_CURRENT = -1;

        public const int DEFAULT_YEAR_ORIGIN = 2023;
        public const int DEFAULT_START_VALUE = -1;

        static int _currentOriginYear = DEFAULT_YEAR_ORIGIN;
        static long _currentValue = DEFAULT_START_VALUE;
        static long _timeBits = GetTimeBits(DateTime.MinValue);  //long.MinValue;
        static bool _isRandomized = false;

        public const int RANDOM_ID_MAX = 8191;  // 13-bit
        public const long RANDOM_ID_BITMASK = 0b_0001_1111_1111_1111L;
        readonly static RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        readonly static byte[] _randomBytes = new byte[1];


        // properties
        public static int CurrentOriginYear => _currentOriginYear;  // for test


        ///<summary>Initialize time bits with default value and last set or default origin year.</summary>
        public static void Init() => Init(DEFAULT_START_VALUE, YEAR_USE_CURRENT);
        public static void Init(int startValue = DEFAULT_START_VALUE, int originYear = YEAR_USE_CURRENT)
        {
            _isRandomized = false;
            _currentValue = Math.Max(DEFAULT_START_VALUE, startValue);

            if (originYear > YEAR_USE_CURRENT)
            {
                var yearOffset = DateTime.UtcNow.Year - originYear;
                _currentOriginYear = yearOffset < 0 || yearOffset > YEAR_MAX
                    ? DEFAULT_YEAR_ORIGIN : originYear;
            }

            SetCreationTime(DateTime.MinValue);
        }


        ///<param name="utcTime">Non-UTC time is automatically converted to UTC time.</param>
        public static long GetTimeBits(DateTime utcTime)
        {
            if (utcTime == DateTime.MinValue)
                utcTime = DateTime.UtcNow;
            if (utcTime.Kind != DateTimeKind.Utc)
                utcTime = utcTime.ToUniversalTime();

            // check origin
            if (utcTime.Year < _currentOriginYear)
                throw new Exception("cannot set creation time before origin year.");

            // datetime in UPPER bits.
            long ret = (long)(utcTime.Year - _currentOriginYear) << 57;
            ret |= (long)utcTime.Month << 53;
            ret |= (long)utcTime.Day << 48;
            ret |= (long)utcTime.Hour << 43;
            ret |= (long)utcTime.Minute << 37;
            ret |= (long)utcTime.Second << 31;
            ret |= (long)utcTime.Millisecond << 21;
            return ret;
        }

        ///<param name="utcTime">Non-UTC time is automatically converted to UTC time.</param>
        public static void SetCreationTime(DateTime utcTime)
        {
            _timeBits = GetTimeBits(utcTime);
        }


        ///<summary>Generate sequential Half-ULID value.</summary>
        ///<remarks>Init() is automatically called when maximum id for current creation time is reached.</remarks>
        public static long Next(int offset = 1)
        {
            _currentValue += Math.Max(1, offset);
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
            return _timeBits | ((long)_randomBytes[0] << 13) | _currentValue;
        }


        ///<returns>Value part of Half-ULID.</returns>
        public static int ToHUlidValue(this long val) => GetValue(val);
        ///<returns>Value part of Half-ULID.</returns>
        public static int GetValue(long val)
        {
            return unchecked((int)(val & 0b_1111_1111_1111_1111_1111_1L));
        }


        ///<returns>DateTime.MinValue when error.</returns>
        public static DateTime ToHUlidDateTime(this long val, int originYear = YEAR_USE_CURRENT)
            => GetDateTime(val, originYear);

        ///<returns>DateTime.MinValue when error.</returns>
        public static DateTime GetDateTime(long val, int originYear = YEAR_USE_CURRENT)
        {
            if (originYear <= YEAR_USE_CURRENT)
                originYear = _currentOriginYear;

            try
            {
                // NOTE: bit operator `>>>` cannot be used in C# 9.0
                //       need to do sign-bit aware operation.
                var year = val >> 57;
                if (year < 0)
                {
                    year &= 0b_0111_1111L;
                }
                year += originYear;

                unchecked
                {
                    return new DateTime(
                        (int)year,
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
                return DateTime.MinValue;
            }
        }


    }
}
