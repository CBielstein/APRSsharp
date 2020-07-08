using System;
using Xunit;
using APaRSer;

namespace APaRSerUnitTests
{
   // [TestClass]
    public class PacketNmeaDataUnitTests
    {
        [Fact]
        public void GetType_FromIdentifier_1()
        {
            Assert.Equal(Packet.NmeaData.Type.GGA, Packet.NmeaData.GetType("GGA"));
        }

        [Fact]
        public void GetType_FromIdentifier_2()
        {
            Assert.Equal(Packet.NmeaData.Type.Unknown, Packet.NmeaData.GetType("POO"));
        }

        [Fact]
        public void GetType_FromIdentifier_3()
        {
            Assert.Equal(Packet.NmeaData.Type.WPT, Packet.NmeaData.GetType("wpt"));
            Assert.Equal(Packet.NmeaData.Type.WPT, Packet.NmeaData.GetType("wpl"));
        }

        [Fact(Skip = "24 Fix skipped tests from old repository")]
        public void GetType_FromRawString_4()
        {
            Assert.Equal(Packet.NmeaData.Type.RMC, Packet.NmeaData.GetType("$GPRMC,063909,A,3349.4302,N,11700.3721,W,43.022,89.3,291099,13.6,E*52"));
        }
    }
}
