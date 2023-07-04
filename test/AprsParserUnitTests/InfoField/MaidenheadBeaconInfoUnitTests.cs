namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using System.Linq;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="MaidenheadBeaconInfo"/> class related to encode/decode of the information field.
    /// </summary>
    public class MaidenheadBeaconInfoUnitTests
    {
        /// <summary>
        /// Verifies decoding and re-encoding a full status packet in TNC2 format.
        /// </summary>
        [Fact]
        public void TestRoundTrip()
        {
            string encoded = "N0CALL>WIDE1-1,WIDE2-2:[IO91SX]35 miles NNW of London";
            Packet p = new Packet(encoded);

            Assert.Equal("N0CALL", p.Sender);
            Assert.Equal(2, p.Path.Count);
            Assert.Equal("WIDE1-1", p.Path.First());
            Assert.Equal("WIDE2-2", p.Path.Last());
            Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1));
            Assert.Equal(PacketType.MaidenheadGridLocatorBeacon, p.InfoField.Type);

            if (p.InfoField is MaidenheadBeaconInfo mbi)
            {
                Assert.NotNull(mbi.Position);

                // Coordinates not precise when coming from gridhead
                double latitude = mbi?.Position?.Coordinates.Latitude ?? throw new Exception("Latitude should not be null");
                double longitude = mbi?.Position?.Coordinates.Longitude ?? throw new Exception("Longitude should not be null");
                Assert.Equal(51.98, Math.Round(latitude, 2));
                Assert.Equal(-0.46, Math.Round(longitude, 2));

                Assert.Equal("IO91SX", mbi?.Position?.EncodeGridsquare(6, false));
                Assert.Equal('\\', mbi?.Position?.SymbolTableIdentifier);
                Assert.Equal('.', mbi?.Position?.SymbolCode);
                Assert.Equal("35 miles NNW of London", mbi?.Comment);
            }
            else
            {
                Assert.IsType<MaidenheadBeaconInfo>(p.InfoField);
            }

            Assert.Equal(encoded, p.EncodeString(Packet.Format.TNC2));
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
        [InlineData("35 miles NNW of London", "[IO91SX]35 miles NNW of London")] // With comment
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
        [InlineData(" 35 miles NNW of London", "[IO91SX] 35 miles NNW of London")] // With comment
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
