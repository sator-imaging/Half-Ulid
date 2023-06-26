#if UNITY_EDITOR
using SatorImaging.HUlid;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Threading;

namespace SatorImaging.Tests.HUlid
{
    public class HalfUlidTests
    {
        const string FMT_TIME = "yyyy-MM-dd HH:mm:ss.fff";

        static void VerboseLog(string msg, long val, int originYear = HalfUlid.YEAR_USE_CURRENT, string suffix = "")
        {
            Debug.Log($"{msg} \t {val,32} \t {val.ToHUlidDateTime(originYear).ToString(FMT_TIME)}  ( yr:{HalfUlid.CurrentOriginYear} query:{originYear} ) \t Value: {val.ToHUlidValue(),-10} \t {suffix}");
        }

        static long NextAndLog(string msg, int offset = -1, int originYear = HalfUlid.YEAR_USE_CURRENT)
        {
            var ret = HalfUlid.Next(offset);
            if (string.IsNullOrWhiteSpace(msg))
                VerboseLog($"{($"Next({offset}): "),-12}", ret, originYear);
            else
                VerboseLog($"{msg} \t {($"Next({offset}): "),12}", ret, originYear);
            return ret;
        }

        static long RandomAndLog(string msg, int originYear = HalfUlid.YEAR_USE_CURRENT)
        {
            var ret = HalfUlid.Random();
            var seq = ret & HalfUlid.RANDOM_ID_BITMASK;
            if (string.IsNullOrWhiteSpace(msg))
                VerboseLog($"{("Random(): "),-12}", ret, originYear, "Sequential Value: " + seq);
            else
                VerboseLog($"{msg} \t {("Random(): "),12}", ret, originYear, "Sequential Value: " + seq);
            return ret;
        }



        [Test]
        [Order(-100)]
        public void BeforeInitializationTest()
        {
            NextAndLog("# Generate Half-ULID before Init()\n");
        }


        [Test]
        public void GeneratorTest()
        {
            long val;

            HalfUlid.Init();
            Debug.Log("# Init()");

            val = NextAndLog("");
            Assert.That(HalfUlid.GetValue(val) == 0);

            val = NextAndLog("");
            Assert.That(HalfUlid.GetValue(val) == 1);

            val = NextAndLog("");
            Assert.That(HalfUlid.GetValue(val) == 2);


            Debug.Log("\n# Init(100)");
            HalfUlid.Init(100);

            val = NextAndLog("");
            Assert.That(HalfUlid.GetValue(val) == 101);

            val = NextAndLog("");
            Assert.That(HalfUlid.GetValue(val) == 102);

            val = NextAndLog("");
            Assert.That(HalfUlid.GetValue(val) == 103);

            //random
            val = RandomAndLog("");
            Assert.That((val & HalfUlid.RANDOM_ID_BITMASK) == 104);
            val = RandomAndLog("");
            Assert.That((val & HalfUlid.RANDOM_ID_BITMASK) == 105);
            val = RandomAndLog("");
            Assert.That((val & HalfUlid.RANDOM_ID_BITMASK) == 106);


            //next() again
            val = NextAndLog("", 10);
            Assert.That(HalfUlid.GetValue(val) == 116);

            val = NextAndLog("", 10);
            Assert.That(HalfUlid.GetValue(val) == 126);

            val = NextAndLog("", 10);
            Assert.That(HalfUlid.GetValue(val) == 136);
        }


        [Test]
        public void InitializeTest()
        {
            long val;

            //originYear
            Debug.Log("# Init() with originYear option");


            HalfUlid.Init(originYear: DateTime.UtcNow.Year - 999);
            val = NextAndLog($"Init(originYear: {DateTime.UtcNow.Year - 999})");
            Assert.That(HalfUlid.CurrentOriginYear == DateTime.UtcNow.Year);

            HalfUlid.Init(originYear: DateTime.UtcNow.Year - 128);
            val = NextAndLog($"Init(originYear: {DateTime.UtcNow.Year - 128})");
            Assert.That(HalfUlid.CurrentOriginYear == DateTime.UtcNow.Year);

            HalfUlid.Init(originYear: DateTime.UtcNow.Year - 127);
            val = NextAndLog($"Init(originYear: {DateTime.UtcNow.Year - 127})");
            Assert.That(HalfUlid.CurrentOriginYear == DateTime.UtcNow.Year - 127);

            HalfUlid.Init(originYear: DateTime.UtcNow.Year - 126);
            val = NextAndLog($"Init(originYear: {DateTime.UtcNow.Year - 126})");
            Assert.That(HalfUlid.CurrentOriginYear == DateTime.UtcNow.Year - 126);


            var lastOriginYear = HalfUlid.CurrentOriginYear;
            HalfUlid.Init(originYear: HalfUlid.YEAR_USE_CURRENT);
            val = NextAndLog($"Init(originYear: {HalfUlid.YEAR_USE_CURRENT})");
            Assert.That(HalfUlid.CurrentOriginYear == lastOriginYear);


            HalfUlid.Init(originYear: DateTime.UtcNow.Year - 1);
            val = NextAndLog($"Init(originYear: {DateTime.UtcNow.Year - 1})");
            Assert.That(HalfUlid.CurrentOriginYear == DateTime.UtcNow.Year - 1);

            HalfUlid.Init(originYear: DateTime.UtcNow.Year);
            val = NextAndLog($"Init(originYear: {DateTime.UtcNow.Year})");
            Assert.That(HalfUlid.CurrentOriginYear == DateTime.UtcNow.Year);

            HalfUlid.Init(originYear: DateTime.UtcNow.Year + 1);
            val = NextAndLog($"Init(originYear: {DateTime.UtcNow.Year + 1})");
            Assert.That(HalfUlid.CurrentOriginYear == DateTime.UtcNow.Year);

            HalfUlid.Init(originYear: DateTime.UtcNow.Year + 310);
            val = NextAndLog($"Init(originYear: {DateTime.UtcNow.Year + 310})");
            Assert.That(HalfUlid.CurrentOriginYear == DateTime.UtcNow.Year);


            //use current
            Debug.Log("\n# HalfUlid.YEAR_USE_CURRENT to use last set value");

            HalfUlid.Init(originYear: 2000);
            HalfUlid.Init();
            Debug.Log($"Init(originYear: 2000) -> Init():      \t\t Origin Year: " + HalfUlid.CurrentOriginYear);
            Assert.That(HalfUlid.CurrentOriginYear == 2000);

            HalfUlid.Init(originYear: 2000);
            HalfUlid.Init(originYear: 0);
            Debug.Log($"Init(originYear: 2000) -> Init(yr:0):    \t Origin Year: " + HalfUlid.CurrentOriginYear);
            Assert.That(HalfUlid.CurrentOriginYear == HalfUlid.DEFAULT_YEAR_ORIGIN);

            HalfUlid.Init(originYear: 2000);
            HalfUlid.Init(originYear: 310);
            Debug.Log($"Init(originYear: 2000) -> Init(yr:310):   \t Origin Year: " + HalfUlid.CurrentOriginYear);
            Assert.That(HalfUlid.CurrentOriginYear == HalfUlid.DEFAULT_YEAR_ORIGIN);

            HalfUlid.Init(originYear: 2000);
            HalfUlid.Init(originYear: 9999);
            Debug.Log($"Init(originYear: 2000) -> Init(yr:9999):   \t Origin Year: " + HalfUlid.CurrentOriginYear);
            Assert.That(HalfUlid.CurrentOriginYear == HalfUlid.DEFAULT_YEAR_ORIGIN);


            //others
            Debug.Log("\n# ToHUlidDateTime() with originYear option");

            HalfUlid.Init(int.MinValue, 2000);
            var year2000 = NextAndLog("Init(int.MinValue, 2000)");
            var year2000dt = year2000.ToHUlidDateTime();

            HalfUlid.Init(int.MaxValue, 2023);
            var year2023 = NextAndLog("Init(int.MaxValue, 2023)");
            var year2023dt = year2023.ToHUlidDateTime();

            Assert.That(year2000dt.Year == year2023dt.Year);
            Assert.That(year2000.ToHUlidDateTime(2000).Year == year2023.ToHUlidDateTime(2023).Year);
            Assert.That(year2000.ToHUlidDateTime().Year != year2023.ToHUlidDateTime().Year);


            VerboseLog("Origin Year 2000: ", year2000, 2000);
            VerboseLog("Origin Year 2023: ", year2023, 2023);

            VerboseLog("Origin Year 2000 w/o query yr: ", year2000);
            VerboseLog("Origin Year 2023 w/o query yr: ", year2023);
        }


        [Test]
        public void SetCreationTimeTest()
        {
            var localNow = DateTime.Now;
            var utcFromLocal = localNow.ToUniversalTime();
            var unixEpoch = DateTime.UnixEpoch;

            Assert.That(localNow.Kind == DateTimeKind.Local);
            Assert.That(utcFromLocal.Kind == DateTimeKind.Utc);
            Assert.That(unixEpoch.Kind == DateTimeKind.Utc);


            //conversion
            HalfUlid.SetCreationTime(localNow);
            var localVal = HalfUlid.Next();

            HalfUlid.SetCreationTime(utcFromLocal);
            var utcVal = HalfUlid.Next();

            Assert.That(localVal.ToHUlidDateTime() == utcVal.ToHUlidDateTime());        // utc conversion must match


            //future
            var future = DateTime.UtcNow.AddYears(100).AddMonths(100).AddDays(100).AddHours(100).AddMinutes(100).AddMilliseconds(1234);
            HalfUlid.SetCreationTime(future);
            var futureVal = HalfUlid.Next();

            //minValue
            HalfUlid.SetCreationTime(DateTime.MinValue);
            var minDateTimeVal = HalfUlid.Next();

            Assert.That(futureVal.ToHUlidDateTime() != minDateTimeVal.ToHUlidDateTime());  //minValue use current time


            //exception
            Assert.Throws<Exception>(() => HalfUlid.SetCreationTime(unixEpoch));
            var epochVal = HalfUlid.Next();

            // creation time cannot be set before origin year.
            Assert.That(epochVal.ToHUlidDateTime() == minDateTimeVal.ToHUlidDateTime());


            VerboseLog("Local Time:  ", localVal);
            VerboseLog("To UTC Time: ", utcVal);
            VerboseLog("Future:      ", futureVal);

            Debug.Log("\n# Cannot set to Unix Epoch time (exception thrown). creation time must be same.");
            VerboseLog("MinValue:   ", minDateTimeVal);
            VerboseLog("Unix Epoch: ", epochVal);
        }


        [Test]
        public void ConverterTest()
        {
            var val = NextAndLog("# Test value\n");

            Debug.Log("Value Extension: \t" + val.ToHUlidValue());
            Debug.Log("Date Extension: \t" + val.ToHUlidDateTime().ToString(FMT_TIME));

            Debug.Log("Value Method: \t" + HalfUlid.GetValue(val));
            Debug.Log("Date Method: \t" + HalfUlid.GetDateTime(val).ToString(FMT_TIME));

            Debug.Log("\n# GetTimeBits(DateTime.MinValue)\nValue: \t" + HalfUlid.GetTimeBits(DateTime.MinValue).ToHUlidValue());
            Debug.Log("Date: \t" + HalfUlid.GetTimeBits(DateTime.MinValue).ToHUlidDateTime().ToString(FMT_TIME));


            // what happens??
            Debug.Log("\n# Non-HUlid values can be converted?  DateTime.MinValue will be returned when error.");

            VerboseLog("-100: ", (-100L));
            Assert.That((-100L).ToHUlidDateTime() == DateTime.MinValue);

            VerboseLog(" -10: ", (-10L));
            Assert.That((-10L).ToHUlidDateTime() == DateTime.MinValue);

            VerboseLog("   0: ", (0L));
            Assert.That((0L).ToHUlidDateTime() == DateTime.MinValue);

            VerboseLog("  10: ", (10L));
            Assert.That((10L).ToHUlidDateTime() == DateTime.MinValue);

            VerboseLog(" 100: ", (100L));
            Assert.That((100L).ToHUlidDateTime() == DateTime.MinValue);
        }


        [Test]
        public void RestartSequenceTest()
        {
            Debug.Log("# Init(HalfUlid.ID_MAX - 2)");
            HalfUlid.Init(HalfUlid.ID_MAX - 2);
            var val1 = NextAndLog("");
            var val2 = NextAndLog("");
            //except restart
            Thread.Sleep(100);  // need to wait
            var val3 = NextAndLog("");
            NextAndLog("");
            NextAndLog("");

            Assert.That(val1.ToHUlidDateTime() == val2.ToHUlidDateTime());
            Assert.That(val1.ToHUlidDateTime() != val3.ToHUlidDateTime());



            // offset 100
            Debug.Log("\n# Init(HalfUlid.ID_MAX - 234)");
            HalfUlid.Init(HalfUlid.ID_MAX - 234);
            val1 = NextAndLog("", 111);
            val2 = NextAndLog("", 111);
            //except restart
            Thread.Sleep(100);  // need to wait
            val3 = NextAndLog("", 111);
            NextAndLog("", 111);
            NextAndLog("", 111);

            Assert.That(val1.ToHUlidDateTime() == val2.ToHUlidDateTime());
            Assert.That(val1.ToHUlidDateTime() != val3.ToHUlidDateTime());


            // random
            Debug.Log("\n# Init(HalfUlid.RANDOM_ID_MAX - 2)");
            HalfUlid.Init(HalfUlid.RANDOM_ID_MAX - 2);
            val1 = RandomAndLog("");
            val2 = RandomAndLog("");
            //except restart
            Thread.Sleep(100);  // need to wait
            val3 = RandomAndLog("");
            RandomAndLog("");
            RandomAndLog("");

            Assert.That(val1.ToHUlidDateTime() == val2.ToHUlidDateTime());
            Assert.That(val1.ToHUlidDateTime() != val3.ToHUlidDateTime());


            // next after random
            Debug.Log("\n# Init(HalfUlid.RANDOM_ID_MAX - 2)");
            HalfUlid.Init(HalfUlid.RANDOM_ID_MAX - 2);
            val1 = RandomAndLog("");
            val2 = RandomAndLog("");
            //except restart
            Thread.Sleep(100);  // need to wait
            val3 = NextAndLog("");
            NextAndLog("");
            NextAndLog("");

            Assert.That(val1.ToHUlidDateTime() == val2.ToHUlidDateTime());
            Assert.That(val1.ToHUlidDateTime() != val3.ToHUlidDateTime());


            // random after next
            Debug.Log("\n# Init(HalfUlid.RANDOM_ID_MAX + 310)");
            HalfUlid.Init(HalfUlid.RANDOM_ID_MAX + 310);
            val1 = NextAndLog("");
            val2 = NextAndLog("");
            //except restart
            Thread.Sleep(100);  // need to wait
            val3 = RandomAndLog("");
            RandomAndLog("");
            RandomAndLog("");

            Assert.That(val1.ToHUlidDateTime() == val2.ToHUlidDateTime());
            Assert.That(val1.ToHUlidDateTime() != val3.ToHUlidDateTime());
        }


    }
}
#endif
