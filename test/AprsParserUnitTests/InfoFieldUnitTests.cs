namespace AprsSharpUnitTests.Parsers.Aprs
{
    using AprsSharp.Parsers.Aprs;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="InfoField"/> class related to encode/decode of the information field.
    /// </summary>
    public class InfoFieldUnitTests
    {
        // NOTE: Many of these are testing incomplete functionality.
        // Any catch of System.NotImplementedException should be considered for removal in the future.
/*
        /// <summary>
        /// Dcodes a positionless weather report based on the example given in the APRS spec.
        /// </summary>
        [Fact]
        public void DecodePositionlessWeatherReportFormat()
        {
            Packet p = new Packet();

            Assert.Throws<NotImplementedException>(() => p.DecodeInformationField("_10090556c220s004g005t077r000p000P000h50b09900wRSW"));

            Assert.Equal(PacketType.WeatherReport, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(TimestampType.MDHM, ts!.DecodedType);
            Assert.Equal(10, ts!.DateTime.Month);
            Assert.Equal(9, ts!.DateTime.Day);
            Assert.Equal(05, ts!.DateTime.Hour);
            Assert.Equal(56, ts!.DateTime.Minute);
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

            Assert.Equal(PacketType.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(TimestampType.DHMz, ts!.DecodedType);
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

            Assert.Equal(PacketType.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp? ts = p.Timestamp;
            Assert.NotNull(ts);
            Assert.Equal(TimestampType.DHMz, ts!.DecodedType);
            Assert.Equal(09, ts!.DateTime.Day);
            Assert.Equal(23, ts!.DateTime.Hour);
            Assert.Equal(45, ts!.DateTime.Minute);

            Assert.True(false, "Not yet handling weather or compressed lat long position.");
        }
*/

        /// <summary>
        /// Tests GetDataType.
        /// </summary>
        /// <param name="informationField">Input information field to test.</param>
        /// <param name="expectedDataType">Expected data type result.</param>
        [Theory]
        [InlineData("/092345z4903.50N/07201.75W>Test1234", PacketType.PositionWithTimestampNoMessaging)]
        [InlineData(">IO91SX/G", PacketType.Status)]
        public void GetDataType(
            string informationField,
            PacketType expectedDataType)
        {
            InfoField infoField = InfoField.FromString(informationField);
            Assert.Equal(expectedDataType, infoField.Type);
        }

/*
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

            string encoded = p.EncodeInformationField(PacketType.MaidenheadGridLocatorBeacon);

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

            string encoded = p.EncodeInformationField(PacketType.MaidenheadGridLocatorBeacon);

            Assert.Equal(expectedEncoding, encoded);
        }*/
    }
}
