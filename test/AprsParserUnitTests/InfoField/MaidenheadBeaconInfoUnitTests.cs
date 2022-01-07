namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="MaidenheadBeaconInfo"/> class related to encode/decode of the information field.
    /// </summary>
    public class MaidenheadBeaconInfoUnitTests
    {
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
            MaidenheadBeaconInfo mbi = new MaidenheadBeaconInfo(informationField);
            Assert.NotNull(mbi.Position);
            Assert.Equal(51.98, Math.Round(mbi.Position.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(mbi.Position.Coordinates.Longitude, 2));
            Assert.Equal(expectedComment, mbi.Comment);
        }

        /// <summary>
        /// Tests encoding a Maidenhead Locator Beacon based on examples from the APRS spec.
        /// </summary>
        /// <param name="comment">Packet comment.</param>
        /// <param name="expectedEncoding">Expected encoding.</param>
        [Theory]
        [InlineData("35 miles NNW of London", "[IO91SX] 35 miles NNW of London")] // With comment
        [InlineData("", "[IO91SX]")] // Without comment
        public void EncodeMaidenheadLocatorBeaconFromLatLong(
            string comment,
            string expectedEncoding)
        {
            Position p = new Position(new GeoCoordinate(51.98, -0.46));
            MaidenheadBeaconInfo mbi = new MaidenheadBeaconInfo(p, comment);
            Assert.Equal(expectedEncoding, mbi.Encode());
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
            Position p = new Position();
            p.DecodeMaidenhead("IO91SX");
            MaidenheadBeaconInfo mbi = new MaidenheadBeaconInfo(p, comment);
            Assert.Equal(expectedEncoding, mbi.Encode());
        }
    }
}
