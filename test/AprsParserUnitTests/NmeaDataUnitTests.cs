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
        /// Tests GetType.
        /// </summary>
        [Fact]
        public void GetType_FromIdentifier_1()
        {
            Assert.Equal(NmeaData.Type.GGA, NmeaData.GetType("GGA"));
        }

        /// <summary>
        /// Tests GetType.
        /// </summary>
        [Fact]
        public void GetType_FromIdentifier_2()
        {
            Assert.Equal(NmeaData.Type.Unknown, NmeaData.GetType("POO"));
        }

        /// <summary>
        /// Tests GetType.
        /// </summary>
        [Fact]
        public void GetType_FromIdentifier_3()
        {
            Assert.Equal(NmeaData.Type.WPT, NmeaData.GetType("wpt"));
            Assert.Equal(NmeaData.Type.WPT, NmeaData.GetType("wpl"));
        }

        /// <summary>
        /// Tests GetType.
        /// </summary>
        [Fact(Skip = "Issue #24: Fix skipped tests from old repository")]
        public void GetType_FromRawString_4()
        {
            Assert.Equal(NmeaData.Type.RMC, NmeaData.GetType("$GPRMC,063909,A,3349.4302,N,11700.3721,W,43.022,89.3,291099,13.6,E*52"));
        }
    }
}
