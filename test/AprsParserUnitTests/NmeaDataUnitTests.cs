namespace AprsSharpUnitTests.Parsers.Aprs
{
    using AprsSharp.Parsers.Aprs;
    using Xunit;

    /// <summary>
    /// Tests NmeaData code.
    /// </summary>
    public class NmeaDataUnitTests
    {
        /// <summary>
        /// Tests GetType from a type identifier.
        /// </summary>
        /// <param name="expected">Expected NmeaData.Type returned by test.</param>
        /// <param name="input">Input string to test.</param>
        [Theory]
        [InlineData(NmeaData.Type.GGA, "GGA")]
        [InlineData(NmeaData.Type.Unknown, "POO")]
        [InlineData(NmeaData.Type.WPT, "wpt")]
        [InlineData(NmeaData.Type.WPT, "wpl")]
        public void GetTypeFromIdentifier(NmeaData.Type expected, string input)
        {
            Assert.Equal(expected, NmeaData.GetType(input));
        }

        /// <summary>
        /// Tests GetType from a full NMEA string.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void GetTypeFromRawString()
        {
            Assert.Equal(NmeaData.Type.RMC, NmeaData.GetType("$GPRMC,063909,A,3349.4302,N,11700.3721,W,43.022,89.3,291099,13.6,E*52"));
        }
    }
}
