namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using System.Linq;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="StatusInfo"/> class related to encode/decode of position reports.
    /// </summary>
    public class StatusInfoUnitTests
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

            Assert.Equal(encoded, p.EncodeTnc2());
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
        [InlineData(">IO91SX/- ^B7", 51.98, -0.46, '-', "^B7", Skip = "Issue #69: Packet decode does not handle Meteor Scatter beam information")]
        public void DecodeStatusReportFormatWithMaidenhead(
            string informationField,
            double expectedLatitute,
            double expectedLongitute,
            char expectedSymbolCode,
            string? expectedComment)
        {
            StatusInfo si = new StatusInfo(informationField);

            if (si.Position == null)
            {
                Assert.NotNull(si.Position);
            }
            else
            {
                Assert.Equal(expectedLatitute, Math.Round(si.Position.Coordinates.Latitude, 2));
                Assert.Equal(expectedLongitute, Math.Round(si.Position.Coordinates.Longitude, 2));
                Assert.Equal('/', si.Position.SymbolTableIdentifier);
                Assert.Equal(expectedSymbolCode, si.Position.SymbolCode);
            }

            Assert.Equal(expectedComment, si.Comment);
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
            Position p = new Position(new GeoCoordinate(51.98, -0.46), '/', 'G', ambiguity);
            StatusInfo si = new StatusInfo(p, comment);
            Assert.Equal(expectedEncoding, si.Encode());
        }

        /// <summary>
        /// Tests decoding a status report without Maidenhead info field based on the APRS spec.
        /// </summary>
        /// <param name="informationField">Information field for decode.</param>
        /// <param name="expectedTime">Expected decoded timestamp.</param>
        /// <param name="expectedComment">Expected decoded comment.</param>
        [Theory]
        [InlineData(">Net Control Center", null, "Net Control Center")]
        [InlineData(">092345zNet Control Center", "092345z", "Net Control Center")]
        public void DecodeStatusReportFormatWithoutMaidenhead(
            string informationField,
            string? expectedTime,
            string? expectedComment)
        {
            StatusInfo si = new StatusInfo(informationField);

            Assert.Null(si.Position);
            Assert.Equal(expectedComment, si.Comment);

            if (expectedTime != null)
            {
                var expectedTimestamp = new Timestamp(expectedTime);
                Assert.NotNull(si.Timestamp);
                Assert.Equal(expectedTimestamp.DecodedType, si.Timestamp!.DecodedType);
                Assert.Equal(expectedTimestamp.DateTime, si.Timestamp!.DateTime);
            }
            else
            {
                Assert.Null(si.Timestamp);
            }
        }

        /// <summary>
        /// Tests encoding a status report without Maidenhead info field based on the APRS spec.
        /// </summary>
        /// <param name="time">Time to encode.</param>
        /// <param name="comment">Comment to encode.</param>
        /// <param name="expectedEncoding">Expected encoding.</param>
        [Theory]
        [InlineData(null, "Net Control Center", ">Net Control Center")]
        [InlineData("092345z", "Net Control Center", ">092345zNet Control Center")]
        public void EncodeStatusReportFormatWithoutMaidenhead(
            string? time,
            string comment,
            string expectedEncoding)
        {
            Timestamp? timestamp = time == null ? null : new Timestamp(time);
            StatusInfo si = new StatusInfo(timestamp, comment);
            Assert.Equal(expectedEncoding, si.Encode());
        }
    }
}
