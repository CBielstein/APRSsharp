using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APaRSer;

namespace APaRSerUnitTests
{
    [TestClass]
    public class PacketNmeaDataUnitTests
    {
        [TestMethod]
        public void GetType_FromIdentifier_1()
        {
            Assert.AreEqual(Packet.NmeaData.Type.GGA, Packet.NmeaData.GetType("GGA"));
        }

        [TestMethod]
        public void GetType_FromIdentifier_2()
        {
            Assert.AreEqual(Packet.NmeaData.Type.Unknown, Packet.NmeaData.GetType("POO"));
        }

        [TestMethod]
        public void GetType_FromIdentifier_3()
        {
            Assert.AreEqual(Packet.NmeaData.Type.WPT, Packet.NmeaData.GetType("wpt"));
            Assert.AreEqual(Packet.NmeaData.Type.WPT, Packet.NmeaData.GetType("wpl"));
        }

        [TestMethod]
        public void GetType_FromRawString_4()
        {
            Assert.AreEqual(Packet.NmeaData.Type.RMC, Packet.NmeaData.GetType("$GPRMC,063909,A,3349.4302,N,11700.3721,W,43.022,89.3,291099,13.6,E*52"));
        }
    }
}
