namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="PositionInfo"/> class related to encode/decode of position reports.
    /// </summary>
    public class PositionInfoUnitTests
    {
       /// <summary>
        /// Verifies decoding and re-encoding a full position packet in TNC2 format.
        /// </summary>
        [Fact]
        public void TestRoundTrip()
        {
            string encoded = "N0CALL>WIDE1-1,igate,TCPIP*:/092345z4903.50N/07201.75W>Test1234";
            Packet p = new Packet(encoded);

            Assert.Equal("N0CALL", p.Sender);
            Assert.Equal(3, p.Path.Count);
            Assert.Equal("WIDE1-1", p.Path[0]);
            Assert.Equal("igate", p.Path[1]);
            Assert.Equal("TCPIP*", p.Path[2]);
            Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1));
            Assert.Equal(PacketType.PositionWithTimestampNoMessaging, p.InfoField.Type);

            if (p.InfoField is PositionInfo pi)
            {
                Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pi?.Position?.Coordinates);
                Assert.Equal('/', pi?.Position?.SymbolTableIdentifier);
                Assert.Equal('>', pi?.Position?.SymbolCode);

                // This is using day/hour/minute encoding, so only check those
                Assert.Equal(9, pi?.Timestamp?.DateTime.Day);
                Assert.Equal(23, pi?.Timestamp?.DateTime.Hour);
                Assert.Equal(45, pi?.Timestamp?.DateTime.Minute);
                Assert.Equal(TimestampType.DHMz, pi?.Timestamp?.DecodedType);
                Assert.Equal(DateTimeKind.Utc, pi?.Timestamp?.DateTime.Kind);

                Assert.Equal("Test1234", pi?.Comment);
                Assert.False(pi?.HasMessaging);
            }
            else
            {
                Assert.IsType<PositionInfo>(p.InfoField);
            }

            Assert.Equal(encoded, p.Encode());
        }

        /// <summary>
        /// Decodes a lat/long position report format with timestamp and comment
        /// based on the example given in the APRS spec.
        /// </summary>
        /// <param name="informationField">The information field to decode.</param>
        /// <param name="expectedPacketType">Expected <see cref="PacketType"/> of the packet.</param>
        /// <param name="expectedHasMessaging">Expected hasMessaging value.</param>
        /// <param name="expectedTimestampType">Expected <see cref="TimestampType"/>.</param>
        [Theory]
        [InlineData("/092345z4903.50N/07201.75W>Test1234", PacketType.PositionWithTimestampNoMessaging, false, TimestampType.DHMz)] // No messaging, UTC
        [InlineData("@092345/4903.50N/07201.75W>Test1234", PacketType.PositionWithTimestampWithMessaging, true, TimestampType.DHMl)] // With messaging, local time
        public void DecodeLatLongPositionReportFormatWithTimestamp(
            string informationField,
            PacketType expectedPacketType,
            bool expectedHasMessaging,
            TimestampType expectedTimestampType)
        {
            PositionInfo pi = new PositionInfo(informationField);

            Assert.Equal(expectedPacketType, pi.Type);
            Assert.Equal(expectedHasMessaging, pi.HasMessaging);

            Assert.NotNull(pi.Timestamp);
            Assert.Equal(expectedTimestampType, pi.Timestamp?.DecodedType);
            Assert.Equal(9, pi.Timestamp?.DateTime.Day);
            Assert.Equal(23, pi.Timestamp?.DateTime.Hour);
            Assert.Equal(45, pi.Timestamp?.DateTime.Minute);

            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pi.Position.Coordinates);
            Assert.Equal('/', pi.Position.SymbolTableIdentifier);
            Assert.Equal('>', pi.Position.SymbolCode);

            Assert.Equal("Test1234", pi.Comment);
        }

        /// <summary>
        /// Encodes a lat/long position report format with timestamp
        /// based on the examples given in the APRS spec.
        /// </summary>
        /// <param name="dateTimeKind">The kind of date/time to use.</param>
        /// <param name="timestampType">The <see cref="TimestampType"/>.</param>
        /// <param name="hasMessaging">If the packet should include messaging.</param>
        /// <param name="packetType"><see cref="PacketType"/> of packet to encode.</param>
        /// <param name="expectedEncoding">The expected encoded string.</param>
        [Theory]
        [InlineData(DateTimeKind.Local, TimestampType.DHMl, true, PacketType.PositionWithTimestampWithMessaging, @"@092345/4903.50N/07201.75W>Test1234")] // Local, has messaging
        [InlineData(DateTimeKind.Utc, TimestampType.DHMz, false, PacketType.PositionWithTimestampNoMessaging, @"/092345z4903.50N/07201.75W>Test1234")] // UTC, no messaging
        public void EncodeLatLongPositionReportFormatWithTimestamp(
            DateTimeKind dateTimeKind,
            TimestampType timestampType,
            bool hasMessaging,
            PacketType packetType,
            string expectedEncoding)
        {
            DateTime dt = new DateTime(2016, 12, 9, 23, 45, 0, 0, dateTimeKind);
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);

            PositionInfo pi = new PositionInfo(
                new Position(gc, '/', '>', 0),
                hasMessaging,
                new Timestamp(dt),
                "Test1234");

            Assert.Equal(packetType, pi.Type);
            Assert.Equal(expectedEncoding, pi.Encode(timestampType));
        }

        /// <summary>
        /// Lat/Long Position Report Format - with Data Extension and Timestamp
        /// based on the examples given in the APRS spec.
        /// </summary>
        /// <param name="informationField">Information field for decoding.</param>
        /// <param name="expectedPacketType">Expected decoded PacketType value.</param>
        /// <param name="expectedHasMessaging">Expected decoded HasMessaging value.</param>
        /// <param name="expectedTimestampType">Expected decoded TimestampType value.</param>
        /// <param name="expectedTimeDay">Expected decoded Timestamp day value.</param>
        /// <param name="expectedTimeHour">Expected decoded Timestamp hour value.</param>
        /// <param name="expectedTimeMinute">Expected decoded Timestamp minute value.</param>
        /// <param name="expecteTimeSecond">Expected decoded Timestamp second value.</param>
        [Theory(Skip = "Issue #24: Fix skipped tests from old repository")]
        [InlineData( // with timestamp, with APRS messaging, local time, course/speed
            "@092345/4903.50N/07201.75W>088/036",
            PacketType.PositionWithTimestampWithMessaging,
            true,
            TimestampType.DHMl,
            9,
            23,
            45,
            0)]
        [InlineData( // with timestamp, APRS messaging, hours/mins/secs time, PHG
            "@234517h4903.50N/07201.75W>PHG5132",
            PacketType.PositionWithTimestampWithMessaging,
            true,
            TimestampType.HMS,
            null,
            23,
            45,
            17)]
        [InlineData( // with timestamp, APRS messaging, zulu time, radio range
            "@092345z4903.50N/07201.75W>RNG0050",
            PacketType.PositionWithTimestampWithMessaging,
            true,
            TimestampType.DHMz,
            9,
            23,
            45,
            0)]
        [InlineData( // with timestamp, hours/mins/secs time, DF, no APRS messaging
            "/234517h4903.50N/07201.75W>DFS2360",
            PacketType.PositionWithTimestampNoMessaging,
            false,
            TimestampType.HMS,
            null,
            23,
            45,
            17)]
        [InlineData(// weather report
            "@092345z4903.50N/07201.75W_090/000g000t066r000p000...dUII",
            PacketType.WeatherReport,
            false,
            TimestampType.DHMz,
            9,
            23,
            45,
            0,
            Skip = "Issue #67: Packet.GetDataType does not support complex data types")]
        public void DecodeLatLongPositionReportFormatWithDataExtensionAndTimestamp(
            string informationField,
            PacketType expectedPacketType,
            bool expectedHasMessaging,
            TimestampType expectedTimestampType,
            int? expectedTimeDay,
            int expectedTimeHour,
            int expectedTimeMinute,
            int expecteTimeSecond)
        {
            PositionInfo pi = new PositionInfo(informationField);

            Assert.Equal(expectedPacketType, pi.Type);
            Assert.Equal(expectedHasMessaging, pi.HasMessaging);

            Assert.NotNull(pi.Timestamp);
            Assert.Equal(expectedTimestampType, pi.Timestamp?.DecodedType);

            if (expectedTimeDay != null)
            {
                Assert.Equal(expectedTimeDay, pi.Timestamp?.DateTime.Day);
            }

            Assert.Equal(expectedTimeHour, pi.Timestamp?.DateTime.Hour);
            Assert.Equal(expectedTimeMinute, pi.Timestamp?.DateTime.Minute);
            Assert.Equal(expecteTimeSecond, pi.Timestamp?.DateTime.Second);

            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pi.Position.Coordinates);
            Assert.Equal('/', pi.Position.SymbolTableIdentifier);
            Assert.Equal('>', pi.Position.SymbolCode);

            Assert.True(false, "Not yet handling data extension.");
        }

        /// <summary>
        /// DF Report Format - with Timestamp
        /// based on the examples given in the APRS spec.
        /// </summary>
        /// <param name="informationField">Information field to decode.</param>
        /// <param name="expectedPacketType">Expected decoded PacketType value.</param>
        /// <param name="expectedHasMessaging">Expected decoded HasMessaging value.</param>
        [Theory(Skip = "Issue #24: Fix skipped tests from old repository")]
        [InlineData( // with timestamp, course/speed/bearing/NRQ, with APRS messaging
            @"@092345z4903.50N/07201.75W\088/036/270/729",
            PacketType.PositionWithTimestampWithMessaging,
            true)]
        [InlineData( // with timestamp, bearing/NRQ, no course/speed, no APRS messaging
            @"/092345z4903.50N/07201.75W\000/000/270/729",
            PacketType.PositionWithTimestampNoMessaging,
            false)]
        public void DecodeDFReportFormat(
            string informationField,
            PacketType expectedPacketType,
            bool expectedHasMessaging)
        {
            PositionInfo pi = new PositionInfo(informationField);

            Assert.Equal(expectedPacketType, pi.Type);
            Assert.Equal(expectedHasMessaging, pi.HasMessaging);

            Assert.NotNull(pi.Timestamp);
            Assert.Equal(TimestampType.DHMz, pi.Timestamp?.DecodedType);
            Assert.Equal(09, pi.Timestamp?.DateTime.Day);
            Assert.Equal(23, pi.Timestamp?.DateTime.Hour);
            Assert.Equal(45, pi.Timestamp?.DateTime.Minute);

            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pi.Position.Coordinates);
            Assert.Equal('/', pi.Position.SymbolTableIdentifier);
            Assert.Equal('\\', pi.Position.SymbolCode);

            Assert.True(false, "Not yet handling DF Report, bearing, course, and/or speed.");
        }

        /// <summary>
        /// Compressed Lat/Long Position Report Format - with Timestamp
        /// with APRS messaging, timestamp, radio range
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeCompressedLatLongPositionReportFormat()
        {
            PositionInfo pi = new PositionInfo("@092345z/5L!!<*e7>{?!");

            Assert.Equal(PacketType.PositionWithTimestampWithMessaging, pi.Type);
            Assert.True(pi.HasMessaging);

            Assert.NotNull(pi.Timestamp);
            Assert.Equal(TimestampType.DHMz, pi.Timestamp?.DecodedType);
            Assert.Equal(09, pi.Timestamp?.DateTime.Day);
            Assert.Equal(23, pi.Timestamp?.DateTime.Hour);
            Assert.Equal(45, pi.Timestamp?.DateTime.Minute);

            Assert.True(false, "Not yet handling compressed latlong position report format.");
        }

        /// <summary>
        /// Complete Lat/Long Position Report Format - without Timestamp
        /// based on the example given in the APRS spec.
        /// </summary>
        /// <param name="informationField">Information field for decode.</param>
        /// <param name="expectedComment">Expected decoded comment.</param>
        /// <param name="expectedLatitute">Expected decoded latitute.</param>
        /// <param name="expectedLongitute">Expected decoded longitude.</param>
        /// <param name="expectedAmbiguity">Expected decoded ambiguity.</param>
        [Theory]
        [InlineData("!4903.50N/07201.75W-Test 001234", "Test 001234", 49.0583, -72.0292, 0)] // no timestamp, no APRS messaging, with comment
        [InlineData("!49  .  N/072  .  W-", null, 49, -72, 4)] // no timestamp, no APRS messaging, location to nearest degree
        [InlineData("!4903.50N/07201.75W-Test /A=001234", "Test /A=001234", 49.0583, -72.0292, 0, Skip = "Issue #68: Packet decode does not handle altitude data")]
        public void DecodeCompleteLatLongPositionReportFormatWithoutTimestamp(
            string informationField,
            string? expectedComment,
            double expectedLatitute,
            double expectedLongitute,
            int expectedAmbiguity)
        {
            PositionInfo pi = new PositionInfo(informationField);

            Assert.Equal(PacketType.PositionWithoutTimestampNoMessaging, pi.Type);
            Assert.False(pi.HasMessaging);
            Assert.Equal(expectedComment, pi.Comment);

            Assert.NotNull(pi.Position);
            Assert.Equal(new GeoCoordinate(expectedLatitute, expectedLongitute), pi.Position?.Coordinates);
            Assert.Equal('/', pi.Position?.SymbolTableIdentifier);
            Assert.Equal('-', pi.Position?.SymbolCode);
            Assert.Equal(expectedAmbiguity, pi.Position?.Ambiguity);
        }

        /// <summary>
        /// Test encoding complete Lat/Long Position Report Format
        /// based on the example given in the APRS spec.
        /// </summary>
        /// <param name="comment">Packet comment.</param>
        /// <param name="ambiguity">Positional ambiguity.</param>
        /// <param name="expectedEncoding">Expected encoding result.</param>
        [Theory]
        [InlineData("Test 001234", 0, @"!4903.50N/07201.75W-Test 001234")] // With comment, no ambiguity.
        [InlineData("", 4, "!49  .  N/072  .  W-")] // Without comment, with ambiguity to the nearest degree.
        public void EncodeCompleteLatLongPositionReportFormatWithoutTimestamp(
            string comment,
            int ambiguity,
            string expectedEncoding)
        {
            PositionInfo pi = new PositionInfo(
                new Position(new GeoCoordinate(49.0583, -72.0292), '/', '-', ambiguity),
                false,
                null,
                comment);

            Assert.Equal(PacketType.PositionWithoutTimestampNoMessaging, pi.Type);

            Assert.Equal(expectedEncoding, pi.Encode());
        }
    }
}