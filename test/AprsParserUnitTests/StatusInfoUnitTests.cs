namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="StatusInfo"/> class related to encode/decode of position reports.
    /// </summary>
    public class StatusInfoUnitTests
    {
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
    }
}
