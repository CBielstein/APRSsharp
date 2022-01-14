namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using System.Linq;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests full decode then encode in <see cref="Packet"/>.
    /// </summary>
    public class PacketRoundTripUnitTests
    {
        /// <summary>
        /// Verifies decoding a full status packet in TNC2 format.
        /// </summary>
        [Fact]
        public void TestFullStatusDecode()
        {
            string encoded = "N0CALL>WIDE2-2:>IO91SX/G My house";
            Packet p = new Packet(encoded);

            Assert.Equal("N0CALL", p.Sender);
            Assert.Equal("WIDE2-2", p.Path.Single());
            Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1));
            Assert.Equal(PacketType.Status, p.InfoField.Type);

            if (p.InfoField is StatusInfo si)
            {
                Assert.Equal(new GeoCoordinate(51.98, -0.46), si?.Position?.Coordinates);
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
        /// Verifies decoding a full position packet in TNC2 format.
        /// </summary>
        [Fact]
        public void TestFullPositionDecode()
        {
            string encoded = "N0CALL>WIDE1-1,igate,TCPIP*:/092345z4903.50N/07201.75W>Test1234";
            Packet p = new Packet(encoded);

            Assert.Equal("N0CALL", p.Sender);
            Assert.Equal(3, p.Path.Count);
            Assert.Equal("WIDE1-1", p.Path[0]);
            Assert.Equal("igate", p.Path[1]);
            Assert.Equal("TCPIP*", p.Path[2]);
            Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1));
            Assert.Equal(PacketType.PositionWithTimestampNoMessaging, p.InfoField.Type);

            if (p.InfoField is PositionInfo pi)
            {
                Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pi?.Position?.Coordinates);
                Assert.Equal('/', pi?.Position?.SymbolTableIdentifier);
                Assert.Equal('>', pi?.Position?.SymbolCode);
                Assert.Equal(new DateTime(2016, 12, 9, 23, 45, 0, 0, DateTimeKind.Utc), pi?.Timestamp?.DateTime);
                Assert.Equal("Test1234", pi?.Comment);
                Assert.False(pi?.HasMessaging);
            }
            else
            {
                Assert.IsType<PositionInfo>(p.InfoField);
            }

            Assert.Equal(encoded, p.Encode());
        }
    }
}
