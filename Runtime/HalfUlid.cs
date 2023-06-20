using System;
using System.Collections.Generic;
using System.Threading;

namespace SatorImaging.HUlid
{
    public static class HalfUlid
    {
        const int ID_MAX = 2097151;  // 2 ^ 21 - 1;
        const int DEFAULT_YEAR_ORIGIN = 2023;
        const int DEFAULT_START_VALUE = -1;

        static int _currentOriginYear = DEFAULT_YEAR_ORIGIN;
        static long _currentValue = DEFAULT_START_VALUE;
        static long _timeBits = GetTimeBits(DateTime.MinValue);  //long.MinValue;
        readonly static Random _rng = new Random();


        //public static void InitTimeBits()
        //{
        //    Interlocked.CompareExchange(ref _timeBits, GetTimeBits(DateTime.MinValue), long.MinValue);
        //}

        public static void Init(int startValue = DEFAULT_START_VALUE, int originYear = DEFAULT_YEAR_ORIGIN)
        {
            _currentValue = Math.Max(DEFAULT_START_VALUE, startValue);
            _currentOriginYear = originYear;
            SetCreationTime(DateTime.MinValue);
        }


        public static long GetTimeBits(DateTime utcTime)
        {
            if (utcTime == DateTime.MinValue)
                utcTime = DateTime.UtcNow;
            if (utcTime.Kind != DateTimeKind.Utc)
                utcTime = utcTime.ToUniversalTime();

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
            //InitTimeBits();
            if (_currentValue == ID_MAX)
                Init(DEFAULT_START_VALUE, _currentOriginYear);

            _currentValue += Math.Max(1, offset);
            return _timeBits | _currentValue;
        }

        ///<summary>Generate Half-ULID value with random number.</summary>
        ///<remarks>NOTE: identical value could be returned for same creation time.</remarks>
        [Obsolete("This method does NOT check conflict to values generated before.")]
        public static long Random()
        {
            //InitTimeBits();
            return _timeBits | (long)_rng.Next(0, ID_MAX);
        }


        public static int ToHUlidValue(this long val) => GetValue(val);
        public static int GetValue(long val)
        {
            return unchecked((int)(val & 0b_1111_1111_1111_1111_1111_1L));
        }

        ///<summary>NOTE: return DateTime.MinValue when error.</summary>
        public static DateTime ToHUlidDateTime(this long val, int originYear = -1) => GetDateTime(val, originYear);
        ///<summary>NOTE: return DateTime.MinValue when error.</summary>
        public static DateTime GetDateTime(long val, int originYear = -1)
        {
            if (originYear < 0)
                originYear = _currentOriginYear;

            try
            {
                unchecked
                {
                    return new DateTime(
                        (int)  /**/ (val >> 57) + originYear              /**/,
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
