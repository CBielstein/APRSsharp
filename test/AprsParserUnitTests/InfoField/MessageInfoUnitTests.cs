namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using System.Linq;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="MessageInfo"/> class related to encode/decode of position reports.
    /// </summary>
    public class MessageInfoUnitTests
    {
        /// <summary>
        /// Verifies decoding and re-encoding a full status packet in TNC2 format.
        /// </summary>
        [Fact]
        public void TestRoundTrip()
        {
            string encoded = "N0CALL>WIDE2-2:>IO91SX/G My house";
            Packet p = new Packet(encoded);

            Assert.Equal("N0CALL", p.Sender);
            Assert.Equal("WIDE2-2", p.Path.Single());
            Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1));
            Assert.Equal(PacketType.Status, p.InfoField.Type);

            if (p.InfoField is StatusInfo si)
            {
                // Coordinates not precise when coming from gridhead
                double latitude = si?.Position?.Coordinates.Latitude ?? throw new Exception("Latitude should not be null");
                double longitude = si?.Position?.Coordinates.Longitude ?? throw new Exception("Longitude should not be null");
                Assert.Equal(51.98, Math.Round(latitude, 2));
                Assert.Equal(-0.46, Math.Round(longitude, 2));
                Assert.Equal("IO91SX", si?.Position?.EncodeGridsquare(6, false));
                Assert.Equal('/', si?.Position?.SymbolTableIdentifier);
                Assert.Equal('G', si?.Position?.SymbolCode);
                Assert.Equal("My house", si?.Comment);
            }
            else
            {
                Assert.IsType<StatusInfo>(p.InfoField);
            }

            Assert.Equal(encoded, p.Encode());
        }

        /// <summary>
        /// Tests decoding a message info field based on the APRS spec.
        /// </summary>
        /// <param name="informationField">Information field for decode.</param>
        /// <param name="expectedAddressee">Expected decoded addressee.</param>
        /// <param name="expectedContent">Expected decoded content.</param>
        /// <param name="expectedId">Expected decoded message ID.</param>
        [Theory]
        [InlineData(":WU2Z     :Testing", "WU2Z", "Testing", null)]
        [InlineData(":WU2Z     :Testing{003", "WU2Z", "Testing", "003")]
        [InlineData(":EMAIL    :msproul@ap.org Test email", "EMAIL", "msproul@ap.org Test email", null)]
        public void DecodeMessageFormat(
            string informationField,
            string expectedAddressee,
            string expectedContent,
            string? expectedId)
        {
            MessageInfo mi = new MessageInfo(informationField);

            Assert.Equal(expectedAddressee, mi.Addressee);
            Assert.Equal(expectedContent, mi.Content);
            Assert.Equal(expectedId, mi.Id);
        }

        /// <summary>
        /// Tests encoding a message info field based on the APRS spec.
        /// </summary>
        /// <param name="addressee">Expected decoded addressee.</param>
        /// <param name="content">Expected decoded content.</param>
        /// <param name="id">Expected decoded message ID.</param>
        /// <param name="expectedEncoding">Information field for decode.</param>
        [Theory]
        [InlineData("WU2Z", "Testing", null, ":WU2Z     :Testing")]
        [InlineData("WU2Z", "Testing", "003", ":WU2Z     :Testing{003")]
        [InlineData("EMAIL", "msproul@ap.org Test email", null, ":EMAIL    :msproul@ap.org Test email")]
        public void EncodeMessageFormat(
            string addressee,
            string content,
            string? id,
            string expectedEncoding)
        {
            MessageInfo mi = new MessageInfo(addressee, content, id);
            Assert.Equal(expectedEncoding, mi.Encode());
        }
    }
}
