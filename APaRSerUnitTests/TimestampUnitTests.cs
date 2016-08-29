using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APaRSer;

namespace APaRSerUnitTests
{
    [TestClass]
    public class TimestampUnitTests
    {
        /// <summary>
        /// Pass a day that is out of the bounds of [1,31] and expect an exception
        /// </summary>
        [TestMethod]
        public void FindCorrectYearAndMonth_DayOutOfRange()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            int outYear = 0;
            int outDay = 0;
            object[] args = new object[] { 32, DateTime.Now, outYear, outDay };

            try
            {
                pts.Invoke("FindCorrectYearAndMonth", args);

                // Should not reach here as an ArgumentOutOfRangeException should be thrown
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentOutOfRangeException));
            }
        }

        /// <summary>
        /// Ensures we find the correct month & year when we're in the same month.
        /// </summary>
        [TestMethod]
        public void FindCorrectYearAndMonth_SameMonth()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            int expectedYear = 2016;
            int expectedMonth = 10;
            DateTime MidMonth = new DateTime(expectedYear, expectedMonth, 24);

            object[] args = new object[] { 7, MidMonth, /* year */ 0, /* month */ 0 };

            pts.Invoke("FindCorrectYearAndMonth", args);

            Assert.AreEqual(expectedYear, args[2]);
            Assert.AreEqual(expectedMonth, args[3]);
        }

        /// <summary>
        /// Ensures we find the correct month & year when the day is from last month.
        /// </summary>
        [TestMethod]
        public void FindCorrectYearAndMonth_PreviousMonth()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            int expectedYear = 2016;
            int expectedMonth = 9;
            DateTime MidMonth = new DateTime(expectedYear, 10, 24);

            object[] args = new object[] { 25, MidMonth, /* year */ 0, /* month */ 0 };

            pts.Invoke("FindCorrectYearAndMonth", args);

            Assert.AreEqual(expectedYear, args[2]);
            Assert.AreEqual(expectedMonth, args[3]);
        }

        /// <summary>
        /// Ensures we find the correct month & year when the day is from last year.
        /// </summary>
        [TestMethod]
        public void FindCorrectYearAndMonth_PreviousYear()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            int expectedYear = 2015;
            int expectedMonth = 12;
            DateTime MidMonth = new DateTime(2016, 1, 1);

            object[] args = new object[] { 31, MidMonth, /* year */ 0, /* month */ 0 };

            pts.Invoke("FindCorrectYearAndMonth", args);

            Assert.AreEqual(expectedYear, args[2]);
            Assert.AreEqual(expectedMonth, args[3]);
        }

        /// <summary>
        /// Ensures we find the correct month & year when the day is from two months ago.
        /// Skips 2/30, since that day doesn't exist.
        /// </summary>
        [TestMethod]
        public void FindCorrectYearAndMonth_TwoMonthsAgo()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            int expectedYear = 2016;
            int expectedMonth = 1;
            DateTime MidMonth = new DateTime(expectedYear, 3, 1);

            object[] args = new object[] { 30, MidMonth, /* year */ 0, /* month */ 0 };

            pts.Invoke("FindCorrectYearAndMonth", args);

            Assert.AreEqual(expectedYear, args[2]);
            Assert.AreEqual(expectedMonth, args[3]);
        }

        /// <summary>
        /// Ensures we find the correct month & year when the day is from two months ago.
        /// Skips 2/29, since that day doesn't exist in 2015.
        /// </summary>
        [TestMethod]
        public void FindCorrectYearAndMonth_NotLeapYear()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            int expectedYear = 2015;
            int expectedMonth = 1;

            DateTime MidMonth = new DateTime(expectedYear, 3, 1);

            object[] args = new object[] { 29, MidMonth, /* year */ 0, /* month */ 0 };

            pts.Invoke("FindCorrectYearAndMonth", args);

            Assert.AreEqual(expectedYear, args[2]);
            Assert.AreEqual(expectedMonth, args[3]);
        }

        /// <summary>
        /// Ensures we find the correct month & year when the day is from two months ago.
        /// Does NOT skip 2/29, since that day DOES exist in 2016.
        /// </summary>
        [TestMethod]
        public void FindCorrectYearAndMonth_LeapYear()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            int expectedYear = 2016;
            int expectedMonth = 2;
            DateTime MidMonth = new DateTime(expectedYear, 3, 1);

            object[] args = new object[] { 29, MidMonth, /* year */ 0, /* month */ 0 };

            pts.Invoke("FindCorrectYearAndMonth", args);

            Assert.AreEqual(expectedYear, args[2]);
            Assert.AreEqual(expectedMonth, args[3]);
        }
    }
}
