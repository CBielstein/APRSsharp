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
        public void FindCorrectYearAndMonthDayOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Timestamp.FindCorrectYearAndMonth(32, DateTime.Now, out _, out _));
        }

        /// <summary>
        /// Tests FindCorrectYearAndMonth.
        /// </summary>
        /// <param name="day">A day to find in the most recent applicable month and year.</param>
        /// <param name="hintYear">The year for the "hint" (usually the current date).</param>
        /// <param name="hintMonth">The month for the "hint" (usually the current date).</param>
        /// <param name="hintDay">The day for the "hint" (usually the current date).</param>
        /// <param name="expectedYear">The expected resultant year.</param>
        /// <param name="expectedMonth">The expected resultant month.</param>
        [Theory]
        [InlineData(7, 2016, 10, 24, 2016, 10)] // Same Month
        [InlineData(25, 2016, 10, 24, 2016, 9)] // Previous Month
        [InlineData(31, 2016, 1, 1, 2015, 12)] // Previous Year
        [InlineData(30, 2016, 3, 1, 2016, 1)] // Two Months Previous
        [InlineData(29, 2015, 3, 1, 2015, 1)] // Not leap year (Feb 29, 2015 does NOT exist)
        [InlineData(29, 2016, 3, 1, 2016, 2)] // Leap year (Feb 29, 2016 DOES exist)
        public void FindCorrectYearAndMonth(
            in int day,
            in int hintYear,
            in int hintMonth,
            in int hintDay,
            in int expectedYear,
            in int expectedMonth)
        {
            DateTime hint = new DateTime(hintYear, hintMonth, hintDay);

            Timestamp.FindCorrectYearAndMonth(day, hint, out int year, out int month);

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
        /// Test FindCorrectDayMonthAndYear from an HMS packet.
        /// This tests all in UTC as all DateTime differences are relative so in theory local vs UTC should not change behavior here.
        /// Hint is 2016/11/1 01:30:00, set offsets appropriately for tests.
        /// </summary>
        /// <param name="minutesBetweenPacketAndHint">Offset the packet time from the hint. Positive values move packet before hint, negative after.</param>
        /// <param name="expectedDaysBeforeHint">Expected number of days before the hint (0 for today, 1 for yesterday).</param>
        [Theory]
        [InlineData(60, 0)] // Same day
        [InlineData(120, 1)] // Yesterday (pushes hint to "tomorrow" relative to packet)
        [InlineData(3, 0)] // Three minutes ahead (test 5 minute packet time buffer)
        public void FindCorrectDayMonthAndYear(
            int minutesBetweenPacketAndHint,
            int expectedDaysBeforeHint)
        {
            DateTime hint = new DateTime(2016, 11, 18, 1, 30, 0, DateTimeKind.Utc);
            DateTime packet = hint.AddMinutes(-1 * minutesBetweenPacketAndHint);

            Timestamp.FindCorrectDayMonthAndYear(
                packet.Hour,
                packet.Minute,
                packet.Second,
                hint,
                out int day,
                out int month,
                out int year);

            Assert.Equal(hint.Day - expectedDaysBeforeHint, day);
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
        /// Test FindCorrectYear.
        /// </summary>
        /// <param name="minutesBetweenPacketAndHint">Offset the packet time from the hint. Positive values move packet before hint, negative after.</param>
        /// <param name="monthsBetweenPacketAndHint">Offset the packet month from the hint. Positive values move packet before hint, negative after.</param>
        /// <param name="expectedYearsBeforeHint">Expected number of days before the hint (0 for this year, 1 for last year, etc.).</param>
        [Theory]
        [InlineData(-3, 0, 0)] // 3 minutes ahead (< 5 minutes) is considered same date
        [InlineData(0, 1, 0)] // Same Year, 1 month ago
        [InlineData(0, 3, 1)] // Last Year, 3 months ago
        public void FindCorrectYear(
            int minutesBetweenPacketAndHint,
            int monthsBetweenPacketAndHint,
            int expectedYearsBeforeHint)
        {
            DateTime hint = new DateTime(2017, 2, 1, 0, 1, 1, DateTimeKind.Utc);
            DateTime packet = hint
                .AddMonths(-1 * monthsBetweenPacketAndHint)
                .AddMinutes(-1 * minutesBetweenPacketAndHint);

            Timestamp.FindCorrectYear(
                packet.Month,
                packet.Day,
                packet.Hour,
                packet.Minute,
                hint,
                out int year);

            Assert.Equal(hint.Year - expectedYearsBeforeHint, year);
        }

        /// <summary>
        /// Test FindCorrectYear leap year edition.
        /// </summary>
        [Fact]
        public void FindCorrectYearInLeapYear()
        {
            DateTime packet = new DateTime(2015, 2, 28, 0, 1, 1, DateTimeKind.Utc);
            DateTime hint = packet.AddMonths(1);

            Timestamp.FindCorrectYear(
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
