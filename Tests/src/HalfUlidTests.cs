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
        static void VerboseLog(string msg, long val, int originYear = -1)
        {
            Debug.Log(msg + "\t" + val.ToString() + " \t " + val.ToHUlidDateTime(originYear).ToString("yyyy-MM-dd HH:mm:ss.fff") + " \t Value: " + val.ToHUlidValue());
        }


        [SetUp]
        public void Setup()
        {
            var val = HalfUlid.Next();
            VerboseLog("HalfUlit.Next() w/o Init(): ", val);
            Debug.Log("");
        }


        [Test]
        public void InitAndGeneratorTest()
        {
            long val;

            HalfUlid.Init(100);
            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 101);
            VerboseLog("Sequential Value: ", val);

            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 102);
            VerboseLog("Sequential Value: ", val);

            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 103);
            VerboseLog("Sequential Value: ", val);

            //random
            val = HalfUlid.Random();
            VerboseLog("    Random Value: ", val);

            val = HalfUlid.Random();
            VerboseLog("    Random Value: ", val);

            val = HalfUlid.Random();
            VerboseLog("    Random Value: ", val);

            //next() again
            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 104);
            VerboseLog("Sequential Value: ", val);

            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 105);
            VerboseLog("Sequential Value: ", val);

            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 106);
            VerboseLog("Sequential Value: ", val);


            //yearOrigin
            HalfUlid.Init(-100, 2000);
            var year2000 = HalfUlid.Next();
            var year2000dt = year2000.ToHUlidDateTime();
            HalfUlid.Init(-100, 2023);
            var year2023 = HalfUlid.Next();
            var year2023dt = year2023.ToHUlidDateTime();

            Assert.That(year2000dt.Year == year2023dt.Year);
            Assert.That(year2000.ToHUlidDateTime(2000).Year == year2023.ToHUlidDateTime(2023).Year);
            Assert.That(year2000.ToHUlidDateTime().Year != year2023.ToHUlidDateTime().Year);

            VerboseLog("Year Origin 2000: ", year2000, 2000);
            VerboseLog("Year Origin 2023: ", year2023, 2023);
        }


        [Test]
        public void SetCreationTimeTest()
        {
            var localNow = DateTime.Now;
            var utcNow = localNow.ToUniversalTime();

            Assert.That(localNow.Kind == DateTimeKind.Local);
            Assert.That(utcNow.Kind == DateTimeKind.Utc);


            HalfUlid.SetCreationTime(localNow);
            var localVal = HalfUlid.Next();

            HalfUlid.SetCreationTime(utcNow);
            var utcVal = HalfUlid.Next();

            Thread.Sleep(100);
            HalfUlid.SetCreationTime(DateTime.MinValue);
            var minDateTimeVal = HalfUlid.Next();

            Assert.That(localVal.ToHUlidDateTime() == utcVal.ToHUlidDateTime());
            Assert.That(minDateTimeVal.ToHUlidDateTime() != utcVal.ToHUlidDateTime());


            VerboseLog("Local Time: ", localVal);
            VerboseLog("UTC Time: ", utcVal);
            VerboseLog("MinValue: ", minDateTimeVal);
        }


        [Test]
        public void ConverterTest()
        {
            var val = HalfUlid.Next();

            Debug.Log("Value Extension: \t" + val.ToHUlidValue());
            Debug.Log(" Date Extension: \t" + val.ToHUlidDateTime());

            Debug.Log("Value Method: \t" + HalfUlid.GetValue(val));
            Debug.Log(" Date Method: \t" + HalfUlid.GetDateTime(val));

            Debug.Log("GetTimeBits value: \t" + HalfUlid.GetTimeBits(DateTime.MinValue).ToHUlidValue());
            Debug.Log(" GetTimeBits date: \t" + HalfUlid.GetTimeBits(DateTime.MinValue).ToHUlidDateTime());


            // what happens??
            Debug.Log("");
            Debug.Log("Non-HUlid values can be converted?");

            VerboseLog("-100: ", (-100L));
            VerboseLog(" -10: ", (-10L));
            VerboseLog("   0: ", (0L));
            VerboseLog("  10: ", (10L));
            VerboseLog(" 100: ", (100L));
        }


        [Test]
        public void AutoRestartSequenceTest()
        {
            HalfUlid.Init(2097149);
            var val1 = HalfUlid.Next();
            var val2 = HalfUlid.Next();
            Thread.Sleep(100);
            var val3 = HalfUlid.Next();

            Assert.That(val1.ToHUlidDateTime() == val2.ToHUlidDateTime());
            Assert.That(val1.ToHUlidDateTime() != val3.ToHUlidDateTime());

            VerboseLog(" Seq 1: ", val1);
            VerboseLog(" Seq 1: ", val2);
            VerboseLog(" Seq 2: ", val3);
        }


    }
}
#endif
