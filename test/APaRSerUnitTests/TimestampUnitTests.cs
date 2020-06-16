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

        /// <summary>
        /// Tests example given in the APRS101.pdf document for zulu time
        /// </summary>
        [TestMethod]
        public void DHMZuluTimeFromSpec()
        {
            Timestamp ts = new Timestamp("092345z");

            Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.AreEqual(9, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);
            Assert.AreEqual(DateTimeKind.Utc, ts.dateTime.Kind);
        }

        /// <summary>
        /// Tests example given in the APRS101.pdf document for local time
        /// </summary>
        [TestMethod]
        public void DHMNotZuluTimeFromSpec()
        {
            Timestamp ts = new Timestamp("092345/");

            Assert.AreEqual(Timestamp.Type.DHMl, ts.DecodedType);
            Assert.AreEqual(9, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);
            Assert.AreEqual(DateTimeKind.Local, ts.dateTime.Kind);
        }

        /// <summary>
        /// Test FindCorrectDayMonthAndYear from an HMS packet perspective
        /// assuming the packet is from the same day using local
        /// </summary>
        [TestMethod]
        public void FindCorrectDayMonthAndYear_SameDayLocal()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            DateTime packet = new DateTime(2016, 11, 18, 22, 48, 16, DateTimeKind.Local);
            DateTime hint = packet.AddHours(1);
            int day = 0;
            int month = 0;
            int year = 0;

            object[] args = new object[] {
                packet.Hour,
                packet.Minute,
                packet.Second,
                hint,
                day,
                month,
                year };

            pts.Invoke("FindCorrectDayMonthAndYear", args);

            Assert.AreEqual(packet.Day, args[4]);
            Assert.AreEqual(packet.Month, args[5]);
            Assert.AreEqual(packet.Year, args[6]);
        }

        /// <summary>
        /// Test FindCorrectDayMonthAndYear from an HMS packet perspective
        /// assuming the packet is from yesterday using zulu
        /// </summary>
        [TestMethod]
        public void FindCorrectDayMonthAndYear_YesterdayZulu()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            DateTime packet = new DateTime(2016, 11, 18, 22, 48, 16, DateTimeKind.Utc);
            DateTime hint = packet.AddHours(-2);
            int day = 0;
            int month = 0;
            int year = 0;

            object[] args = new object[] {
                packet.Hour,
                packet.Minute,
                packet.Second,
                hint,
                day,
                month,
                year };

            pts.Invoke("FindCorrectDayMonthAndYear", args);

            Assert.AreEqual(packet.Day - 1, args[4]);
            Assert.AreEqual(packet.Month, args[5]);
            Assert.AreEqual(packet.Year, args[6]);
        }

        /// <summary>
        /// Test FindCorrectDayMonthAndYear from an HMS packet perspective
        /// with the packet coming from 3 minutes before hint
        /// </summary>
        [TestMethod]
        public void FindCorrectDayMonthAndYear_3MinutesAhead()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            DateTime packet = new DateTime(2017, 1, 1, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMinutes(-3);
            int day = 0;
            int month = 0;
            int year = 0;

            object[] args = new object[] {
                packet.Hour,
                packet.Minute,
                packet.Second,
                hint,
                day,
                month,
                year };

            pts.Invoke("FindCorrectDayMonthAndYear", args);

            Assert.AreEqual(hint.Day, args[4]);
            Assert.AreEqual(hint.Month, args[5]);
            Assert.AreEqual(hint.Year, args[6]);
        }

        /// <summary>
        /// Test HMS with example from APRS101.pdf
        /// </summary>
        [TestMethod]
        public void HMSTestFromSpec()
        {
            Timestamp ts = new Timestamp("234517h");

            Assert.AreEqual(Timestamp.Type.HMS, ts.DecodedType);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);
            Assert.AreEqual(17, ts.dateTime.Second);
        }

        /// <summary>
        /// Test FindCorrectYear's 5 minute drift logic
        /// </summary>
        [TestMethod]
        public void FindCorrectYear_3MinutesAhead()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            DateTime packet = new DateTime(2017, 1, 1, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMinutes(-3);
            int year = 0;

            object[] args = new object[] {
                packet.Month,
                packet.Day,
                packet.Hour,
                packet.Minute,
                hint,
                year };

            pts.Invoke("FindCorrectYear", args);

            Assert.AreEqual(hint.Year, args[5]);
        }

        /// <summary>
        /// Test FindCorrectYear with a packet from earlier this year
        /// </summary>
        [TestMethod]
        public void FindCorrectYear_ThisYear()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            DateTime packet = new DateTime(2016, 11, 18, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMonths(1);
            int year = 0;

            object[] args = new object[] {
                packet.Month,
                packet.Day,
                packet.Hour,
                packet.Minute,
                hint,
                year };

            pts.Invoke("FindCorrectYear", args);

            Assert.AreEqual(packet.Year, args[5]);
        }

        /// <summary>
        /// Test FindCorrectYear with a packet from last year
        /// </summary>
        [TestMethod]
        public void FindCorrectYear_LastYear()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            DateTime packet = new DateTime(2016, 11, 18, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMonths(3);
            int year = 0;

            object[] args = new object[] {
                packet.Month,
                packet.Day,
                packet.Hour,
                packet.Minute,
                hint,
                year };

            pts.Invoke("FindCorrectYear", args);

            Assert.AreEqual(packet.Year, args[5]);
        }

        /// <summary>
        /// Test FindCorrectYear leap year edition
        /// </summary>
        [TestMethod]
        public void FindCorrectYear_LeapYear()
        {
            Timestamp ts = new Timestamp();
            PrivateObject pts = new PrivateObject(ts);

            DateTime packet = new DateTime(2015, 2, 28, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMonths(1);
            int year = 0;

            object[] args = new object[] {
                packet.Month,
                packet.Day + 1,
                packet.Hour,
                packet.Minute,
                hint,
                year };

            pts.Invoke("FindCorrectYear", args);

            Assert.AreEqual(2012, args[5]);
        }

        /// <summary>
        /// Test MDHM with the example from the APRS spec
        /// </summary>
        [TestMethod]
        public void MDHMTestFromSpec()
        {
            Timestamp ts = new Timestamp("10092345");

            Assert.AreEqual(Timestamp.Type.MDHM, ts.DecodedType);
            Assert.AreEqual(10, ts.dateTime.Month);
            Assert.AreEqual(9, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);
        }

        /// <summary>
        /// Encodes in DHM zulu time with the example from the APRS spec
        /// </summary>
        [TestMethod]
        public void EncodeDHMZuluTestFromSpec()
        {
            DateTime dt = new DateTime(2016, 10, 9, 23, 45, 17, DateTimeKind.Utc);
            Timestamp ts = new Timestamp(dt);

            Assert.AreEqual("092345z", ts.Encode(Timestamp.Type.DHMz));
        }

        /// <summary>
        /// Encodes in DHM local time with the example from the APRS spec
        /// </summary>
        [TestMethod]
        public void EncodeDHMLocalTestFromSpec()
        {
            DateTime dt = new DateTime(2016, 10, 9, 23, 45, 17, DateTimeKind.Local);
            Timestamp ts = new Timestamp(dt);

            Assert.AreEqual("092345/", ts.Encode(Timestamp.Type.DHMl));
        }

        /// <summary>
        /// Encodes in HMS with the example from the APRS spec
        /// </summary>
        [TestMethod]
        public void EncodeHMSTestFromSpec()
        {
            DateTime dt = new DateTime(2016, 10, 9, 23, 45, 17, DateTimeKind.Local);
            Timestamp ts = new Timestamp(dt);

            Assert.AreEqual("234517h", ts.Encode(Timestamp.Type.HMS));
        }

        /// <summary>
        /// Encodes in MDHM with the example from the APRS spec
        /// </summary>
        [TestMethod]
        public void EncodeMDHMTestFromSpec()
        {
            DateTime dt = new DateTime(2016, 10, 9, 23, 45, 17, DateTimeKind.Local);
            Timestamp ts = new Timestamp(dt);
            Assert.AreEqual("10092345", ts.Encode(Timestamp.Type.MDHM));
        }
    }
}
