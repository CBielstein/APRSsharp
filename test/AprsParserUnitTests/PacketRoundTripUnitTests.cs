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

                // This is using day/hour/minute encoding, so only check those
                Assert.Equal(9, pi?.Timestamp?.DateTime.Day);
                Assert.Equal(23, pi?.Timestamp?.DateTime.Hour);
                Assert.Equal(45, pi?.Timestamp?.DateTime.Minute);
                Assert.Equal(TimestampType.DHMz, pi?.Timestamp?.DecodedType);
                Assert.Equal(DateTimeKind.Utc, pi?.Timestamp?.DateTime.Kind);

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
