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
        [SetUp]
        public void Setup()
        {
            var val = HalfUlid.Next();
            Debug.Log("Value w/o Init(): " + val.ToHUlidValue());
            Debug.Log(" Time w/o Init(): " + val.ToHUlidDateTime());
            Debug.Log("");
        }


        [Test]
        public void InitAndGeneratorTest()
        {
            long val;

            HalfUlid.Init(100);
            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 101);
            Debug.Log("Sequential Value: " + val + " \t Value: " + val.ToHUlidValue());

            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 102);
            Debug.Log("Sequential Value: " + val + " \t Value: " + val.ToHUlidValue());

            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 103);
            Debug.Log("Sequential Value: " + val + " \t Value: " + val.ToHUlidValue());

            //random
            val = HalfUlid.Random();
            Debug.Log("Random Value: " + val.ToHUlidValue());

            val = HalfUlid.Random();
            Debug.Log("Random Value: " + val.ToHUlidValue());

            val = HalfUlid.Random();
            Debug.Log("Random Value: " + val.ToHUlidValue());

            //next() again
            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 104);
            Debug.Log("Sequential Value: " + val + " \t Value: " + val.ToHUlidValue());

            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 105);
            Debug.Log("Sequential Value: " + val + " \t Value: " + val.ToHUlidValue());

            val = HalfUlid.Next();
            Assert.That(HalfUlid.GetValue(val) == 106);
            Debug.Log("Sequential Value: " + val + " \t Value: " + val.ToHUlidValue());


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

            Debug.Log("Year Origin 2000: " + year2000.ToString());
            Debug.Log("Year Origin 2023: " + year2023.ToString());
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


            Debug.Log("Local: " + localVal + " \t Value: " + localVal.ToHUlidValue());
            Debug.Log("UTC: " + utcVal + " \t Value: " + utcVal.ToHUlidValue());
            Debug.Log("MinValue: " + minDateTimeVal + " \t Value: " + minDateTimeVal.ToHUlidValue());
        }


        [Test]
        public void ConverterTest()
        {
            var val = HalfUlid.Next();

            Debug.Log("Value Extension: " + val.ToHUlidValue());
            Debug.Log(" Date Extension: " + val.ToHUlidDateTime());

            Debug.Log("Value Method: " + HalfUlid.GetValue(val));
            Debug.Log(" Date Method: " + HalfUlid.GetDateTime(val));

            Debug.Log("GetTimeBits value: " + HalfUlid.GetTimeBits(DateTime.MinValue).ToHUlidValue());
            Debug.Log(" GetTimeBits date: " + HalfUlid.GetTimeBits(DateTime.MinValue).ToHUlidDateTime());


            // what happens??
            Debug.Log("");
            Debug.Log("Non-HUlid values can be converted?");

            Debug.Log("-100 value: " + (-100L).ToHUlidValue());
            Debug.Log(" -100 date: " + (-100L).ToHUlidDateTime());
            Debug.Log("-1 value: " + (-1L).ToHUlidValue());
            Debug.Log(" -1 date: " + (-1L).ToHUlidDateTime());
            Debug.Log("0 value: " + (0L).ToHUlidValue());
            Debug.Log(" 0 date: " + (0L).ToHUlidDateTime());
            Debug.Log("1 value: " + (1L).ToHUlidValue());
            Debug.Log(" 1 date: " + (1L).ToHUlidDateTime());
            Debug.Log("100 value: " + (100L).ToHUlidValue());
            Debug.Log(" 100 date: " + (100L).ToHUlidDateTime());
        }


    }
}
#endif
