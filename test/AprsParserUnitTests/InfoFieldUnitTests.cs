namespace AprsSharpUnitTests.Parsers.Aprs
{
    using AprsSharp.Parsers.Aprs;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="InfoField"/> class related to encode/decode of the information field.
    /// </summary>
    public class InfoFieldUnitTests
    {
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
