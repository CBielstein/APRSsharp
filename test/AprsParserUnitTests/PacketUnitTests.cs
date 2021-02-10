namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="Packet"/> class.
    /// </summary>
    public class PacketUnitTests
    {
        // NOTE: Many of these are testing incomplete functionality.
        // Any catch of System.NotImplementedException should be considered for removal in the future.

        /// <summary>
        /// Dcodes a positionless weather report based on the example given in the APRS spec.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_PositionlessWeatherReportFormat()
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
        /// Decodes a lat/long position report format with timestamp, no APRS messaging, zulu time, with comment
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_1()
        {
            Packet p = new Packet();

            p.DecodeInformationField("/092345z4903.50N/07201.75W>Test1234");

            Assert.Equal(Packet.Type.PositionWithTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.DHMz, ts!.DecodedType);
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
        /// Dcodes a lat/long position report format with timestamp, with APRS messaging, local time, with comment
        /// based on the example given in the APRS spec.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_2()
        {
            Packet p = new Packet();

            p.DecodeInformationField("@092345/4903.50N/07201.75W>Test1234");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(Timestamp.Type.DHMl, ts!.DecodedType);
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
        /// Encodes a lat/long position report format with timestamp.
        /// </summary>
        /// <param name="dateTimeKind">The kind of date/time to use.</param>
        /// <param name="timestampType">The timestamp type.</param>
        /// <param name="hasMessaging">If the packet should include messaging.</param>
        /// <param name="packetType">Type of packet to encode.</param>
        /// <param name="expectedEncoding">The expected encoded string.</param>
        [Theory]
        [InlineData(DateTimeKind.Local, Timestamp.Type.DHMl, true, Packet.Type.PositionWithTimestampWithMessaging, @"@092345/4903.50N/07201.75W>Test1234")] // Local, has messaging
        [InlineData(DateTimeKind.Utc, Timestamp.Type.DHMz, false, Packet.Type.PositionWithTimestampNoMessaging, @"/092345z4903.50N/07201.75W>Test1234")] // UTC, no messaging
        public void EncodeInformationFieldFromSpecExampleLatLongPositionReportFormatWithTimestamp(
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
        /// Lat/Long Position Report Format — with Data Extension and Timestamp
        /// with timestamp, with APRS messaging, local time, course/speed.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_1()
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
        /// Lat/Long Position Report Format — with Data Extension and Timestamp
        /// with timestamp, APRS messaging, hours/mins/secs time, PHG.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_2()
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
        /// Lat/Long Position Report Format — with Data Extension and Timestamp
        /// with timestamp, APRS messaging, zulu time, radio range.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_3()
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
        /// Lat/Long Position Report Format — with Data Extension and Timestamp
        /// with timestamp, hours/mins/secs time, DF, no APRS messaging.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_4()
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
        /// Lat/Long Position Report Format — with Data Extension and Timestamp
        /// weather report.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_5()
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
        ///  DF Report Format — with Timestamp
        ///  with timestamp, course/speed/bearing/NRQ, with APRS messaging.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_DFReportFormat_1()
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
        ///  DF Report Format — with Timestamp
        ///   with timestamp, bearing/NRQ, no course/speed, no APRS messaging.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_DFReportFormat_2()
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
        ///  Compressed Lat/Long Position Report Format — with Timestamp
        ///  with APRS messaging, timestamp, radio range.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_CompressedLatLongPositionReportFormat()
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
        ///  Complete Weather Report Format — with Lat/Long position and Timestamp.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_CompleteWeatherReportFormatwithLatLongPositionAndTimestamp()
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
        ///  Complete Weather Report Format — with Compressed Lat/Long position, with Timestamp.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_CompleteWeatherReportFormatWithCompressedLatLongPositionWithTimestamp()
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
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, with comment.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_1()
        {
            Packet p = new Packet();
            p.DecodeInformationField("!4903.50N/07201.75W-Test 001234");

            Assert.Equal(Packet.Type.PositionWithoutTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);
            Assert.Equal("Test 001234", p.Comment);

            Position? pos = p.Position;
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos!.Coordinates);
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('-', pos!.SymbolCode);
        }

        /// <summary>
        /// Test encoding complete Lat/Long Position Report Format.
        /// </summary>
        /// <param name="comment">Packet comment.</param>
        /// <param name="ambiguity">Positional ambiguity.</param>
        /// <param name="expectedEncoding">Expected encoding result.</param>
        [Theory]
        [InlineData("Test 001234", 0, @"!4903.50N/07201.75W-Test 001234")] // With comment, no ambiguity.
        [InlineData("", 4, "!49  .  N/072  .  W-")] // Without comment, with ambiguity to the nearest degree.
        public void EncodeInformationFieldFromSpecExampleCompleteLatLongPositionReportFormatWithoutTimestamp(
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
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, altitude = 1234 ft.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_2()
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
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, location to nearest degree.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_3()
        {
            Packet p = new Packet();
            p.DecodeInformationField("!49  .  N/072  .  W-");

            Assert.Equal(Packet.Type.PositionWithoutTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);
            Assert.Null(p.Comment);

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(new GeoCoordinate(49, -72), pos!.Coordinates);
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('-', pos!.SymbolCode);
            Assert.Equal(4, pos!.Ambiguity);
        }

        /// <summary>
        /// Tests GetTypeChar for <see cref="Packet.Type.DoNotUse"/>.
        /// </summary>
        [Fact]
        public void GetTypeChar_DoNotUse()
        {
            Assert.Throws<ArgumentException>(() => Packet.GetTypeChar(Packet.Type.DoNotUse));
        }

        /// <summary>
        /// Tests GetTypeChar for <see cref="Packet.Type.PositionWithoutTimestampWithMessaging"/>.
        /// </summary>
        [Fact]
        public void GetTypeChar_1()
        {
            Assert.Equal('=', Packet.GetTypeChar(Packet.Type.PositionWithoutTimestampWithMessaging));
        }

        /// <summary>
        /// Tests GetDataType.
        /// </summary>
        [Fact]
        public void GetDataType_1()
        {
            Assert.Equal(
                Packet.Type.PositionWithTimestampNoMessaging,
                Packet.GetDataType("/092345z4903.50N/07201.75W>Test1234"));
        }

        /// <summary>
        /// Tests decoding a Maidenhead Locator Beacon based on the APRS spec.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_MaidenheadLocatorBeacon_1()
        {
            Packet p = new Packet();

            p.DecodeInformationField("[IO91SX] 35 miles NNW of London");

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(51.98, Math.Round(pos!.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos!.Coordinates.Longitude, 2));
            Assert.Equal(" 35 miles NNW of London", p!.Comment);
        }

        /// <summary>
        /// Tests decoding a Maidenhead Locator Beacon based on the APRS spec.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_MaidenheadLocatorBeacon_2()
        {
            Packet p = new Packet();
            p.DecodeInformationField("[IO91SX]");

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(51.98, Math.Round(pos!.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos!.Coordinates.Longitude, 2));
            Assert.Null(p.Comment);
        }

        /// <summary>
        /// Tests encoding a Maidenhead Locator Beacon based on examples from the APRS spec.
        /// </summary>
        /// <param name="comment">Packet comment.</param>
        /// <param name="expectedEncoding">Expected encoding.</param>
        [Theory]
        [InlineData("35 miles NNW of London", "[IO91SX] 35 miles NNW of London")] // With comment
        [InlineData("", "[IO91SX]")] // Without comment
        public void EncodeInformationFieldFromSpecExampleMaidenheadLocatorBeaconFromGps(
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
        public void EncodeInformationFieldFromSpecExampleMaidenheadLocatorBeaconFromMaidenhead(
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
        [Fact]
        public void DecodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_1()
        {
            Packet p = new Packet();
            p.DecodeInformationField(">IO91SX/G");

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(51.98, Math.Round(pos!.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos!.Coordinates.Longitude, 2));
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('G', pos!.SymbolCode);
        }

        /// <summary>
        /// Tests decoding a status report with Maidenhead info field based on the APRS spec.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_2()
        {
            Packet p = new Packet();
            p.DecodeInformationField(">IO91/G");

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(51.5, Math.Round(pos!.Coordinates.Latitude, 2));
            Assert.Equal(-1.0, Math.Round(pos!.Coordinates.Longitude, 2));
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('G', pos!.SymbolCode);
        }

        /// <summary>
        /// Tests decoding a status report with Maidenhead info field based on the APRS spec.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_3()
        {
            Packet p = new Packet();
            p.DecodeInformationField(">IO91SX/- My house");

            Position? pos = p.Position;
            Assert.NotNull(pos);
            Assert.Equal(51.98, Math.Round(pos!.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos!.Coordinates.Longitude, 2));
            Assert.Equal('/', pos!.SymbolTableIdentifier);
            Assert.Equal('-', pos!.SymbolCode);

            string? comment = p.Comment;
            Assert.NotNull(comment);
            Assert.Equal("My house", comment);
        }

        /// <summary>
        /// Tests decoding a status report with Maidenhead info field based on the APRS spec.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_4()
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
        public void EncodeInformationFieldFromSpecExampleStatusReportFormatWithMaidenhead(
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
