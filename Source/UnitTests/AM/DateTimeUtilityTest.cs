using System;

using AM;
using AM.PlatformAbstraction;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable InvokeAsExtensionMethod

namespace UnitTests.AM
{
    [TestClass]
    public class DateTimeUtilityTest
    {
        private PlatformAbstractionLayer _previousLayer;
        private DateTime _nowValue;

        [TestInitialize]
        public void Setup()
        {
            _previousLayer = PlatformAbstractionLayer.Current;
            _nowValue = new DateTime(2019, 10, 9);
            PlatformAbstractionLayer.Current = new TestingPlatformAbstraction
            {
                NowValue = _nowValue
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            PlatformAbstractionLayer.Current = _previousLayer;
        }

        [TestMethod]
        public void DateTimeUtility_NextMonth_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2019, 11, 1),
                    DateTimeUtility.NextMonth
                );
        }

        [TestMethod]
        public void DateTimeUtility_NextYear_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2020, 1, 1),
                    DateTimeUtility.NextYear
                );

        }

        [TestMethod]
        public void DateTimeUtility_PreviousMonth_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2019, 9, 1),
                    DateTimeUtility.PreviousMonth
                );

        }

        [TestMethod]
        public void DateTimeUtility_PreviousYear_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2018, 1, 1),
                    DateTimeUtility.PreviousYear
                );

        }

        [TestMethod]
        public void DateTimeUtility_ThisMonth_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2019, 10, 1),
                    DateTimeUtility.ThisMonth
                );

        }

        [TestMethod]
        public void DateTimeUtility_ThisYear_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2019, 1, 1),
                    DateTimeUtility.ThisYear
                );

        }

        [TestMethod]
        public void DateTimeUtility_Tomorrow_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2019, 10, 10),
                    DateTimeUtility.Tomorrow
                );

        }

        [TestMethod]
        public void DateTimeUtility_Yesterday_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2019, 10, 8),
                    DateTimeUtility.Yesterday
                );

        }

        [TestMethod]
        public void DateTimeUtility_ToJulian_1()
        {
            Assert.AreEqual
                (
                    2458766L,
                    DateTimeUtility.ToJulian(_nowValue)
                );

            Assert.AreEqual
                (
                    2458485L,
                    DateTimeUtility.ToJulian(DateTimeUtility.ThisYear)
                );
        }

        [TestMethod]
        public void DateTimeUtility_FromJulianDate_1()
        {
            Assert.AreEqual
                (
                    "20191022",
                    DateTimeUtility.FromJulianDate(_nowValue)
                );
        }

        [TestMethod]
        public void DateTimeUtility_ToLongUniformString_1()
        {
            Assert.AreEqual
                (
                    "2019-10-09 00:00:00",
                    DateTimeUtility.ToLongUniformString(_nowValue)
                );
        }

        [TestMethod]
        public void DateTimeUtility_ToShortUniformString_1()
        {
            Assert.AreEqual
                (
                    "2019-10-09",
                    DateTimeUtility.ToShortUniformString(_nowValue)
                );
        }

        [TestMethod]
        public void DateTimeUtility_ToUnixDate_1()
        {
            Assert.AreEqual
                (
                1570579200L,
                    DateTimeUtility.ToUnixTime(_nowValue)
                );
        }

        [TestMethod]
        public void DateTimeUtility_Between_1()
        {
            Assert.IsTrue
                (
                    new DateTime(2010, 1, 1).Between
                        (
                            new DateTime(2000, 1, 1),
                            new DateTime(2016, 1, 1)
                        )
                );

            Assert.IsFalse
                (
                    new DateTime(2017, 1, 1).Between
                        (
                            new DateTime(2000, 1, 1),
                            new DateTime(2016, 1, 1)
                        )
                );
        }

        [TestMethod]
        public void DateTimeUtility_MaxDate_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2017, 1, 1),
                    DateTimeUtility.MaxDate
                        (
                            new DateTime(2017, 1, 1)
                        )
                );

            Assert.AreEqual
                (
                    new DateTime(2017, 1, 1),
                    DateTimeUtility.MaxDate
                        (
                            new DateTime(2016, 1, 1),
                            new DateTime(2017, 1, 1)
                        )
                );
        }

        [TestMethod]
        public void DateTimeUtility_MinDate_1()
        {
            Assert.AreEqual
                (
                    new DateTime(2016, 1, 1),
                    DateTimeUtility.MinDate
                        (
                            new DateTime(2016, 1, 1)
                        )
                );

            Assert.AreEqual
                (
                    new DateTime(2016, 1, 1),
                    DateTimeUtility.MinDate
                        (
                            new DateTime(2016, 1, 1),
                            new DateTime(2017, 1, 1)
                        )
                );

            Assert.AreEqual
                (
                    new DateTime(2016, 1, 1),
                    DateTimeUtility.MinDate
                        (
                            new DateTime(2017, 1, 1),
                            new DateTime(2016, 1, 1)
                        )
                );
        }
    }
}
