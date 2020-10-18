namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using AprsSharp.Parsers.Aprs;
    using Xunit;

    /// <summary>
    /// Test Timestamp code.
    /// </summary>
    public class TimestampUnitTests
    {
        /// <summary>
        /// Pass a day that is out of the bounds of [1,31] and expect an exception.
        /// </summary>
        [Fact]
        public void FindCorrectYearAndMonth_DayOutOfRange()
        {
            Timestamp ts = new Timestamp();

            try
            {
                ts.FindCorrectYearAndMonth(32, DateTime.Now, out _, out _);

                // Should not reach here as an ArgumentOutOfRangeException should be thrown
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.IsType<ArgumentOutOfRangeException>(e);
            }
        }

        /// <summary>
        /// Ensures we find the correct month and year when we're in the same month.
        /// </summary>
        [Fact]
        public void FindCorrectYearAndMonth_SameMonth()
        {
            Timestamp ts = new Timestamp();

            int expectedYear = 2016;
            int expectedMonth = 10;
            DateTime midMonth = new DateTime(expectedYear, expectedMonth, 24);

            ts.FindCorrectYearAndMonth(7, midMonth, out int year, out int month);

            Assert.Equal(expectedYear, year);
            Assert.Equal(expectedMonth, month);
        }

        /// <summary>
        /// Ensures we find the correct month and year when the day is from last month.
        /// </summary>
        [Fact]
        public void FindCorrectYearAndMonth_PreviousMonth()
        {
            Timestamp ts = new Timestamp();

            int expectedYear = 2016;
            int expectedMonth = 9;
            DateTime midMonth = new DateTime(expectedYear, 10, 24);

            ts.FindCorrectYearAndMonth(25, midMonth, out int year, out int month);

            Assert.Equal(expectedYear, year);
            Assert.Equal(expectedMonth, month);
        }

        /// <summary>
        /// Ensures we find the correct month and year when the day is from last year.
        /// </summary>
        [Fact]
        public void FindCorrectYearAndMonth_PreviousYear()
        {
            Timestamp ts = new Timestamp();

            int expectedYear = 2015;
            int expectedMonth = 12;
            DateTime midMonth = new DateTime(2016, 1, 1);

            ts.FindCorrectYearAndMonth(31, midMonth, out int year, out int month);

            Assert.Equal(expectedYear, year);
            Assert.Equal(expectedMonth, month);
        }

        /// <summary>
        /// Ensures we find the correct month and year when the day is from two months ago.
        /// Skips 2/30, since that day doesn't exist.
        /// </summary>
        [Fact]
        public void FindCorrectYearAndMonth_TwoMonthsAgo()
        {
            Timestamp ts = new Timestamp();

            int expectedYear = 2016;
            int expectedMonth = 1;
            DateTime midMonth = new DateTime(expectedYear, 3, 1);

            ts.FindCorrectYearAndMonth(30, midMonth, out int year, out int month);

            Assert.Equal(expectedYear,  year);
            Assert.Equal(expectedMonth, month);
        }

        /// <summary>
        /// Ensures we find the correct month and year when the day is from two months ago.
        /// Skips 2/29, since that day doesn't exist in 2015.
        /// </summary>
        [Fact]
        public void FindCorrectYearAndMonth_NotLeapYear()
        {
            Timestamp ts = new Timestamp();

            int expectedYear = 2015;
            int expectedMonth = 1;

            DateTime midMonth = new DateTime(expectedYear, 3, 1);

            ts.FindCorrectYearAndMonth(29, midMonth, out int year, out int month);

            Assert.Equal(expectedYear, year);
            Assert.Equal(expectedMonth, month);
        }

        /// <summary>
        /// Ensures we find the correct month and year when the day is from two months ago.
        /// Does NOT skip 2/29, since that day DOES exist in 2016.
        /// </summary>
        [Fact]
        public void FindCorrectYearAndMonth_LeapYear()
        {
            Timestamp ts = new Timestamp();

            int expectedYear = 2016;
            int expectedMonth = 2;
            DateTime midMonth = new DateTime(expectedYear, 3, 1);

            ts.FindCorrectYearAndMonth(29, midMonth, out int year, out int month);

            Assert.Equal(expectedYear, year);
            Assert.Equal(expectedMonth, month);
        }

        /// <summary>
        /// Tests example given in the APRS101.pdf document for zulu time.
        /// </summary>
        [Fact]
        public void DHMZuluTimeFromSpec()
        {
            Timestamp ts = new Timestamp("092345z");

            Assert.Equal(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.Equal(9, ts.DateTime.Day);
            Assert.Equal(23, ts.DateTime.Hour);
            Assert.Equal(45, ts.DateTime.Minute);
            Assert.Equal(DateTimeKind.Utc, ts.DateTime.Kind);
        }

        /// <summary>
        /// Tests example given in the APRS101.pdf document for local time.
        /// </summary>
        [Fact]
        public void DHMNotZuluTimeFromSpec()
        {
            Timestamp ts = new Timestamp("092345/");

            Assert.Equal(Timestamp.Type.DHMl, ts.DecodedType);
            Assert.Equal(9, ts.DateTime.Day);
            Assert.Equal(23, ts.DateTime.Hour);
            Assert.Equal(45, ts.DateTime.Minute);
            Assert.Equal(DateTimeKind.Local, ts.DateTime.Kind);
        }

        /// <summary>
        /// Test FindCorrectDayMonthAndYear from an HMS packet perspective
        /// assuming the packet is from the same day using local.
        /// </summary>
        [Fact]
        public void FindCorrectDayMonthAndYear_SameDayLocal()
        {
            Timestamp ts = new Timestamp();

            DateTime packet = new DateTime(2016, 11, 18, 22, 48, 16, DateTimeKind.Local);
            DateTime hint = packet.AddHours(1);

            ts.FindCorrectDayMonthAndYear(
                packet.Hour,
                packet.Minute,
                packet.Second,
                hint,
                out int day,
                out int month,
                out int year);

            Assert.Equal(packet.Day, day);
            Assert.Equal(packet.Month, month);
            Assert.Equal(packet.Year, year);
        }

        /// <summary>
        /// Test FindCorrectDayMonthAndYear from an HMS packet perspective
        /// assuming the packet is from yesterday using zulu.
        /// </summary>
        [Fact]
        public void FindCorrectDayMonthAndYear_YesterdayZulu()
        {
            Timestamp ts = new Timestamp();

            DateTime packet = new DateTime(2016, 11, 18, 22, 48, 16, DateTimeKind.Utc);
            DateTime hint = packet.AddHours(-2);

            ts.FindCorrectDayMonthAndYear(
                packet.Hour,
                packet.Minute,
                packet.Second,
                hint,
                out int day,
                out int month,
                out int year);

            Assert.Equal(packet.Day - 1, day);
            Assert.Equal(packet.Month, month);
            Assert.Equal(packet.Year, year);
        }

        /// <summary>
        /// Test FindCorrectDayMonthAndYear from an HMS packet perspective
        /// with the packet coming from 3 minutes before hint.
        /// </summary>
        [Fact]
        public void FindCorrectDayMonthAndYear_3MinutesAhead()
        {
            Timestamp ts = new Timestamp();

            DateTime packet = new DateTime(2017, 1, 1, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMinutes(-3);

            ts.FindCorrectDayMonthAndYear(
                packet.Hour,
                packet.Minute,
                packet.Second,
                hint,
                out int day,
                out int month,
                out int year);

            Assert.Equal(hint.Day, day);
            Assert.Equal(hint.Month, month);
            Assert.Equal(hint.Year, year);
        }

        /// <summary>
        /// Test HMS with example from APRS101.pdf.
        /// </summary>
        [Fact]
        public void HMSTestFromSpec()
        {
            Timestamp ts = new Timestamp("234517h");

            Assert.Equal(Timestamp.Type.HMS, ts.DecodedType);
            Assert.Equal(23, ts.DateTime.Hour);
            Assert.Equal(45, ts.DateTime.Minute);
            Assert.Equal(17, ts.DateTime.Second);
        }

        /// <summary>
        /// Test FindCorrectYear's 5 minute drift logic.
        /// </summary>
        [Fact]
        public void FindCorrectYear_3MinutesAhead()
        {
            Timestamp ts = new Timestamp();

            DateTime packet = new DateTime(2017, 1, 1, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMinutes(-3);

            ts.FindCorrectYear(
                packet.Month,
                packet.Day,
                packet.Hour,
                packet.Minute,
                hint,
                out int year);

            Assert.Equal(hint.Year, year);
        }

        /// <summary>
        /// Test FindCorrectYear with a packet from earlier this year.
        /// </summary>
        [Fact]
        public void FindCorrectYear_ThisYear()
        {
            Timestamp ts = new Timestamp();

            DateTime packet = new DateTime(2016, 11, 18, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMonths(1);

            ts.FindCorrectYear(
                packet.Month,
                packet.Day,
                packet.Hour,
                packet.Minute,
                hint,
                out int year);

            Assert.Equal(packet.Year, year);
        }

        /// <summary>
        /// Test FindCorrectYear with a packet from last year.
        /// </summary>
        [Fact]
        public void FindCorrectYear_LastYear()
        {
            Timestamp ts = new Timestamp();

            DateTime packet = new DateTime(2016, 11, 18, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMonths(3);

            ts.FindCorrectYear(
                packet.Month,
                packet.Day,
                packet.Hour,
                packet.Minute,
                hint,
                out int year);

            Assert.Equal(packet.Year, year);
        }

        /// <summary>
        /// Test FindCorrectYear leap year edition.
        /// </summary>
        [Fact]
        public void FindCorrectYear_LeapYear()
        {
            Timestamp ts = new Timestamp();

            DateTime packet = new DateTime(2015, 2, 28, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMonths(1);

            ts.FindCorrectYear(
                packet.Month,
                packet.Day + 1,
                packet.Hour,
                packet.Minute,
                hint,
                out int year);

            Assert.Equal(2012, year);
        }

        /// <summary>
        /// Test MDHM with the example from the APRS spec.
        /// </summary>
        [Fact]
        public void MDHMTestFromSpec()
        {
            Timestamp ts = new Timestamp("10092345");

            Assert.Equal(Timestamp.Type.MDHM, ts.DecodedType);
            Assert.Equal(10, ts.DateTime.Month);
            Assert.Equal(9, ts.DateTime.Day);
            Assert.Equal(23, ts.DateTime.Hour);
            Assert.Equal(45, ts.DateTime.Minute);
        }

        /// <summary>
        /// Encodes in DHM zulu time with the example from the APRS spec.
        /// </summary>
        [Fact]
        public void EncodeDHMZuluTestFromSpec()
        {
            DateTime dt = new DateTime(2016, 10, 9, 23, 45, 17, DateTimeKind.Utc);
            Timestamp ts = new Timestamp(dt);

            Assert.Equal("092345z", ts.Encode(Timestamp.Type.DHMz));
        }

        /// <summary>
        /// Encodes in DHM local time with the example from the APRS spec.
        /// </summary>
        [Fact]
        public void EncodeDHMLocalTestFromSpec()
        {
            DateTime dt = new DateTime(2016, 10, 9, 23, 45, 17, DateTimeKind.Local);
            Timestamp ts = new Timestamp(dt);

            Assert.Equal("092345/", ts.Encode(Timestamp.Type.DHMl));
        }

        /// <summary>
        /// Encodes in HMS with the example from the APRS spec
        /// The relevant conversion to HMS format includes conversion to UTC.
        /// The <see cref="DateTime"/> object passed to the <see cref="Timestamp"/>
        /// constructor should be converted to the local machine's time zone to
        /// account for the conversion back.
        /// </summary>
        [Fact]
        public void EncodeHMSTestFromSpec()
        {
            DateTime dt = new DateTime(2016, 10, 9, 23, 45, 17, DateTimeKind.Utc);
            Timestamp ts = new Timestamp(dt.ToLocalTime());

            Assert.Equal("234517h", ts.Encode(Timestamp.Type.HMS));
        }

        /// <summary>
        /// Encodes in MDHM with the example from the APRS spec
        /// The relevant conversion to MDHM format includes conversion to UTC.
        /// The <see cref="DateTime"/> object passed to the <see cref="Timestamp"/>
        /// constructor should be converted to the local machine's time zone to
        /// account for the conversion back.
        /// </summary>
        [Fact]
        public void EncodeMDHMTestFromSpec()
        {
            DateTime dt = new DateTime(2016, 10, 9, 23, 45, 17, DateTimeKind.Utc);
            Timestamp ts = new Timestamp(dt.ToLocalTime());
            Assert.Equal("10092345", ts.Encode(Timestamp.Type.MDHM));
        }
    }
}
