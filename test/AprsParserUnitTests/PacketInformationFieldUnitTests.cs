namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="Packet"/> class related to encode/decode of the information field.
    /// </summary>
    public class PacketInformationFieldUnitTests
    {
        // NOTE: Many of these are testing incomplete functionality.
        // Any catch of System.NotImplementedException should be considered for removal in the future.

        /// <summary>
        /// Dcodes a positionless weather report based on the example given in the APRS spec.
        /// </summary>
        [Fact]
        public void DecodePositionlessWeatherReportFormat()
        {
            Packet p = new Packet();

            Assert.Throws<NotImplementedException>(() => p.DecodeInformationField("_10090556c220s004g005t077r000p000P000h50b09900wRSW"));

            Assert.Equal(Packet.Type.WeatherReport, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.MDHM, ts!.DecodedType);
            Assert.Equal(10, ts!.DateTime.Month);
            Assert.Equal(9, ts!.DateTime.Day);
            Assert.Equal(05, ts!.DateTime.Hour);
            Assert.Equal(56, ts!.DateTime.Minute);
        }

        /// <summary>
        /// Decodes a lat/long position report format with timestamp and comment
        /// based on the example given in the APRS spec.
        /// </summary>
        /// <param name="informationField">The information field to decode.</param>
        /// <param name="expectedPacketType">Expected type of the packet.</param>
        /// <param name="expectedHasMessaging">Expected hasMessaging value.</param>
        /// <param name="expectedTimestampType">Expected Timestamp type.</param>
        [Theory]
        [InlineData("/092345z4903.50N/07201.75W>Test1234", Packet.Type.PositionWithTimestampNoMessaging, false, Timestamp.Type.DHMz)] // No messaging, UTC
        [InlineData("@092345/4903.50N/07201.75W>Test1234", Packet.Type.PositionWithTimestampWithMessaging, true, Timestamp.Type.DHMl)] // With messaging, local time
        public void DecodeLatLongPositionReportFormatWithTimestamp(
            string informationField,
            Packet.Type expectedPacketType,
            bool expectedHasMessaging,
            Timestamp.Type expectedTimestampType)
        {
            Packet p = new Packet();

            p.DecodeInformationField(informationField);

            Assert.Equal(expectedPacketType, p.DecodedType);
            Assert.Equal(expectedHasMessaging, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(expectedTimestampType, ts!.DecodedType);
            Assert.Equal(9, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos!.Coordinates);
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('>', pos!.SymbolCode);

            Assert.Equal("Test1234", p.Comment);
        }

        /// <summary>
        /// Encodes a lat/long position report format with timestamp
        /// based on the examples given in the APRS spec.
        /// </summary>
        /// <param name="dateTimeKind">The kind of date/time to use.</param>
        /// <param name="timestampType">The timestamp type.</param>
        /// <param name="hasMessaging">If the packet should include messaging.</param>
        /// <param name="packetType">Type of packet to encode.</param>
        /// <param name="expectedEncoding">The expected encoded string.</param>
        [Theory]
        [InlineData(DateTimeKind.Local, Timestamp.Type.DHMl, true, Packet.Type.PositionWithTimestampWithMessaging, @"@092345/4903.50N/07201.75W>Test1234")] // Local, has messaging
        [InlineData(DateTimeKind.Utc, Timestamp.Type.DHMz, false, Packet.Type.PositionWithTimestampNoMessaging, @"/092345z4903.50N/07201.75W>Test1234")] // UTC, no messaging
        public void EncodeLatLongPositionReportFormatWithTimestamp(
            DateTimeKind dateTimeKind,
            Timestamp.Type timestampType,
            bool hasMessaging,
            Packet.Type packetType,
            string expectedEncoding)
        {
            Packet p = new Packet();

            DateTime dt = new DateTime(2016, 12, 9, 23, 45, 0, 0, dateTimeKind);
            p.Timestamp = new Timestamp(dt);

            p.HasMessaging = hasMessaging;

            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            p.Position = new Position(gc, '/', '>', 0);

            p.Comment = "Test1234";

            string encoded = p.EncodeInformationField(packetType, timestampType);

            Assert.Equal(expectedEncoding, encoded);
        }

        /// <summary>
        /// Lat/Long Position Report Format - with Data Extension and Timestamp
        /// with timestamp, with APRS messaging, local time, course/speed
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeLatLongPositionReportFormatWithDataExtensionAndTimestamp_1()
        {
            Packet p = new Packet();

            p.DecodeInformationField("@092345/4903.50N/07201.75W>088/036");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.DHMl, ts!.DecodedType);
            Assert.Equal(9, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Assert.True(false, "Not yet handling data extension.");
        }

        /// <summary>
        /// Lat/Long Position Report Format - with Data Extension and Timestamp
        /// with timestamp, APRS messaging, hours/mins/secs time, PHG
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeLatLongPositionReportFormatWithDataExtensionAndTimestamp_2()
        {
            Packet p = new Packet();
            p.DecodeInformationField("@234517h4903.50N/07201.75W>PHG5132");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.HMS, ts!.DecodedType);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);
            Assert.Equal(17, ts!.DateTime.Second);

            Assert.True(false, "Not yet handling data extension.");
        }

        /// <summary>
        /// Lat/Long Position Report Format - with Data Extension and Timestamp
        /// with timestamp, APRS messaging, zulu time, radio range
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeLatLongPositionReportFormatWithDataExtensionAndTimestamp_3()
        {
            Packet p = new Packet();

            p.DecodeInformationField("@092345z4903.50N/07201.75W>RNG0050");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.DHMz, ts!.DecodedType);
            Assert.Equal(09, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Assert.True(false, "Not yet handling data extensions.");
        }

        /// <summary>
        /// Lat/Long Position Report Format - with Data Extension and Timestamp
        /// with timestamp, hours/mins/secs time, DF, no APRS messaging
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeLatLongPositionReportFormatWithDataExtensionAndTimestamp_4()
        {
            Packet p = new Packet();
            p.DecodeInformationField("/234517h4903.50N/07201.75W>DFS2360");

            Assert.Equal(Packet.Type.PositionWithTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.HMS, ts!.DecodedType);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);
            Assert.Equal(17, ts!.DateTime.Second);

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos!.Coordinates);
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('>', pos!.SymbolCode);

            Assert.True(false, "Not yet handling DF data.");
        }

        /// <summary>
        /// Lat/Long Position Report Format - with Data Extension and Timestamp
        /// weather report based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeLatLongPositionReportFormatWithDataExtensionAndTimestamp_5()
        {
            Packet p = new Packet();
            p.DecodeInformationField("@092345z4903.50N/07201.75W_090/000g000t066r000p000…dUII");

            Assert.Equal(Packet.Type.WeatherReport, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.DHMz, ts!.DecodedType);
            Assert.Equal(09, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Assert.True(false, "Not yet handling weather reports");
        }

        /// <summary>
        /// DF Report Format - with Timestamp
        /// with timestamp, course/speed/bearing/NRQ, with APRS messaging
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeDFReportFormat_1()
        {
            Packet p = new Packet();
            p.DecodeInformationField("@092345z4903.50N/07201.75W\088/036/270/729");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.DHMz, ts!.DecodedType);
            Assert.Equal(09, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Assert.True(false, "Not yet handling DF Report.");
        }

        /// <summary>
        /// DF Report Format - with Timestamp
        /// with timestamp, bearing/NRQ, no course/speed, no APRS messaging
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeDFReportFormat_2()
        {
            Packet p = new Packet();
            p.DecodeInformationField(@"/092345z4903.50N/07201.75W\000/000/270/729");

            Assert.Equal(Packet.Type.PositionWithTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts!.DecodedType);
            Assert.Equal(09, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos!.Coordinates);
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('\\', pos!.SymbolCode);

            Assert.True(false, "Not yet handling bearing, course/speed.");
        }

        /// <summary>
        /// Compressed Lat/Long Position Report Format - with Timestamp
        /// with APRS messaging, timestamp, radio range
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeCompressedLatLongPositionReportFormat()
        {
            Packet p = new Packet();
            p.DecodeInformationField("@092345z/5L!!<*e7>{?!");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts!.DecodedType);
            Assert.Equal(09, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Assert.True(false, "Not yet handling compressed latlong position report format.");
        }

        /// <summary>
        /// Complete Weather Report Format - with Lat/Long position and Timestamp
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeCompleteWeatherReportFormatwithLatLongPositionAndTimestamp()
        {
            Packet p = new Packet();
            p.DecodeInformationField("@092345z4903.50N/07201.75W_220/004g005t-07r000p000P000h50b09900wRSW");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.DHMz, ts!.DecodedType);
            Assert.Equal(09, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Assert.True(false, "Not yet handling weather information.");
        }

        /// <summary>
        /// Complete Weather Report Format - with Compressed Lat/Long position, with Timestamp
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeCompleteWeatherReportFormatWithCompressedLatLongPositionWithTimestamp()
        {
            Packet p = new Packet();

            p.DecodeInformationField("@092345z/5L!!<*e7 _7P[g005t077r000p000P000h50b09900wRSW");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.DHMz, ts!.DecodedType);
            Assert.Equal(09, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Assert.True(false, "Not yet handling weather or compressed lat long position.");
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
        [InlineData("!4903.50N/07201.75W-Test 001234", "Test 001234", 49.0583, -72.0292,  0)] // no timestamp, no APRS messaging, with comment
        [InlineData("!49  .  N/072  .  W-", null, 49, -72, 4)] // no timestamp, no APRS messaging, location to nearest degree
        public void DecodeCompleteLatLongPositionReportFormatWithoutTimestamp(
            string informationField,
            string? expectedComment,
            double expectedLatitute,
            double expectedLongitute,
            int expectedAmbiguity)
        {
            Packet p = new Packet();
            p.DecodeInformationField(informationField);

            Assert.Equal(Packet.Type.PositionWithoutTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);
            Assert.Equal(expectedComment, p.Comment);

            Position? pos = p.Position;
            Assert.Equal(new GeoCoordinate(expectedLatitute, expectedLongitute), pos!.Coordinates);
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('-', pos!.SymbolCode);
            Assert.Equal(expectedAmbiguity, pos!.Ambiguity);
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
            Packet p = new Packet();
            p.HasMessaging = false;

            p.Comment = comment;
            p.Position = new Position(new GeoCoordinate(49.0583, -72.0292), '/', '-', ambiguity);

            string encoded = p.EncodeInformationField(Packet.Type.PositionWithoutTimestampNoMessaging);
            Assert.Equal(expectedEncoding, encoded);
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format - without Timestamp
        /// no timestamp, no APRS messaging, altitude = 1234 ft
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeCompleteLatLongPositionReportFormatWithoutTimestamp_2()
        {
            Packet p = new Packet();
            p.DecodeInformationField("!4903.50N/07201.75W-Test /A=001234");

            Assert.Equal(Packet.Type.PositionWithoutTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);
            Assert.Equal("Test /A=001234", p.Comment);

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos!.Coordinates);
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('-', pos!.SymbolCode);

            Assert.True(false, "Unhandled altitude.");
        }

        /// <summary>
        /// Tests GetTypeChar for <see cref="Packet.Type.DoNotUse"/>.
        /// </summary>
        [Fact]
        public void GetTypeCharDoNotUseThrows()
        {
            Assert.Throws<ArgumentException>(() => Packet.GetTypeChar(Packet.Type.DoNotUse));
        }

        /// <summary>
        /// Tests GetTypeChar.
        /// </summary>
        /// <param name="packetType">Packet type to test.</param>
        /// <param name="expectedChar">Expected character result.</param>
        [Theory]
        [InlineData(Packet.Type.PositionWithoutTimestampWithMessaging, '=')]
        public void GetTypeChar(
            Packet.Type packetType,
            char expectedChar)
        {
            Assert.Equal(expectedChar, Packet.GetTypeChar(packetType));
        }

        /// <summary>
        /// Tests GetDataType.
        /// </summary>
        /// <param name="informationField">Input information field to test.</param>
        /// <param name="expectedDataType">Expected data type result.</param>
        [Theory]
        [InlineData("/092345z4903.50N/07201.75W>Test1234", Packet.Type.PositionWithTimestampNoMessaging)]
        public void GetDataType(
            string informationField,
            Packet.Type expectedDataType)
        {
            Assert.Equal(
                expectedDataType,
                Packet.GetDataType(informationField));
        }

        /// <summary>
        /// Tests decoding a Maidenhead Locator Beacon based on the APRS spec.
        /// </summary>
        /// <param name="informationField">Information field for decode.</param>
        /// <param name="expectedComment">Expected comment.</param>
        [Theory]
        [InlineData("[IO91SX] 35 miles NNW of London", " 35 miles NNW of London")]
        [InlineData("[IO91SX]", null)]
        public void DecodeMaidenheadLocatorBeacon(string informationField, string? expectedComment)
        {
            Packet p = new Packet();

            p.DecodeInformationField(informationField);

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(51.98, Math.Round(pos!.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos!.Coordinates.Longitude, 2));
            Assert.Equal(expectedComment, p!.Comment);
        }

        /// <summary>
        /// Tests encoding a Maidenhead Locator Beacon based on examples from the APRS spec.
        /// </summary>
        /// <param name="comment">Packet comment.</param>
        /// <param name="expectedEncoding">Expected encoding.</param>
        [Theory]
        [InlineData("35 miles NNW of London", "[IO91SX] 35 miles NNW of London")] // With comment
        [InlineData("", "[IO91SX]")] // Without comment
        public void EncodeMaidenheadLocatorBeaconFromGps(
            string comment,
            string expectedEncoding)
        {
            Packet p = new Packet();
            p.Comment = comment;
            p.Position = new Position(new GeoCoordinate(51.98, -0.46));

            string encoded = p.EncodeInformationField(Packet.Type.MaidenheadGridLocatorBeacon);

            Assert.Equal(expectedEncoding, encoded);
        }

        /// <summary>
        /// Tests encoding a Maidenhead Locator Beacon based on the APRS spec from a Maidenhead position.
        /// </summary>
        /// <param name="comment">Packet comment.</param>
        /// <param name="expectedEncoding">Expected encoding.</param>
        [Theory]
        [InlineData("35 miles NNW of London", "[IO91SX] 35 miles NNW of London")] // With comment
        [InlineData("", "[IO91SX]")] // Without comment
        public void EncodeMaidenheadLocatorBeaconFromMaidenhead(
            string comment,
            string expectedEncoding)
        {
            Packet p = new Packet();
            p.Comment = comment;
            p.Position = new Position();
            p.Position.DecodeMaidenhead("IO91SX");

            string encoded = p.EncodeInformationField(Packet.Type.MaidenheadGridLocatorBeacon);

            Assert.Equal(expectedEncoding, encoded);
        }

        /// <summary>
        /// Tests decoding a status report with Maidenhead info field based on the APRS spec.
        /// </summary>
        /// <param name="informationField">Information field for decode.</param>
        /// <param name="expectedLatitute">Expected decoded latitute value.</param>
        /// <param name="expectedLongitute">Expected decoded longitude value.</param>
        /// <param name="expectedSymbolCode">Expected decoded symbol code value.</param>
        /// <param name="expectedComment">Expected decoded comment.</param>
        [Theory]
        [InlineData(">IO91SX/G", 51.98, -0.46, 'G', null)]
        [InlineData(">IO91/G", 51.5, -1.0, 'G', null)]
        [InlineData(">IO91SX/- My house", 51.98, -0.46, '-', "My house")]
        public void DecodeStatusReportFormatWithMaidenhead(
            string informationField,
            double expectedLatitute,
            double expectedLongitute,
            char expectedSymbolCode,
            string? expectedComment)
        {
            Packet p = new Packet();
            p.DecodeInformationField(informationField);

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(expectedLatitute, Math.Round(pos!.Coordinates.Latitude, 2));
            Assert.Equal(expectedLongitute, Math.Round(pos!.Coordinates.Longitude, 2));
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal(expectedSymbolCode, pos!.SymbolCode);
            Assert.Equal(expectedComment, p.Comment);
        }

        /// <summary>
        /// Tests decoding a status report with Maidenhead info field based on the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeStatusReportFormatWithMaidenhead_4()
        {
            Packet p = new Packet();

            p.DecodeInformationField(">IO91SX/- ^B7");

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(51.98, Math.Round(pos!.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos!.Coordinates.Longitude, 2));
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('-', pos!.SymbolCode);

            string? comment = p.Comment;
            Assert.NotNull(comment);
            Assert.Equal("^B7", comment);

            Assert.True(false, "Not handling Meteor Scatter beam");
        }

        /// <summary>
        /// Tests encoding a status report with Maidenhead info field based on the APRS spec.
        /// </summary>
        /// <param name="comment">Packet comment.</param>
        /// <param name="ambiguity">Position ambiguity.</param>
        /// <param name="expectedEncoding">Expected encoding.</param>
        [Theory]
        [InlineData("", 0, ">IO91SX/G")] // Without comment, no ambiguity
        [InlineData("", 2, ">IO91/G")] // Without comment, with ambiguity
        [InlineData("My house", 0, ">IO91SX/G My house")] // With comment, without ambiguity
        public void EncodeStatusReportFormatWithMaidenhead(
            string comment,
            int ambiguity,
            string expectedEncoding)
        {
            Packet p = new Packet();
            p.Comment = comment;
            p.Position = new Position(new GeoCoordinate(51.98, -0.46), '/', 'G', ambiguity);
            string encoded = p.EncodeInformationField(Packet.Type.Status);

            Assert.Equal(expectedEncoding, encoded);
        }
    }
}
