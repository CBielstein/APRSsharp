namespace AprsSharpUnitTests.AprsParser;

using System;
using AprsSharp.AprsParser;
using Xunit;

/// <summary>
/// Tests <see cref="Packet"/> code for encoding and decoding AX.25 packet format.
/// </summary>
public class PacketAx25UnitTests
{
    /// <summary>
    /// Tests a full roundtrip encode then decode in AX.25.
    /// </summary>
    [Fact]
    public void RoundTripEncodeDecode()
    {
        var sender = "N0CALL";
        var destination = "N0NE";
        var path = new string[2] { "WIDE2-2", "WIDE1-1" };
        var info = new StatusInfo(new Timestamp(DateTime.UtcNow), "Testing 1 2 3!");

        var packet = new Packet(sender, destination, path, info);

        var encoded = packet.EncodeAx25();

        var decodedPacket = new Packet(encoded);

        Assert.Equal(sender, decodedPacket.Sender);
        Assert.Equal(destination, decodedPacket.Destination);
        Assert.Equal(path, decodedPacket.Path);

        var si = Assert.IsType<StatusInfo>(decodedPacket.InfoField);
        Assert.NotNull(si.Timestamp);

        Assert.Equal(DateTimeKind.Utc, si.Timestamp.DateTime.Kind);
        Assert.Equal(info.Timestamp!.DateTime.Day, si.Timestamp.DateTime.Day);
        Assert.Equal(info.Timestamp!.DateTime.Hour, si.Timestamp.DateTime.Hour);
        Assert.Equal(info.Timestamp!.DateTime.Minute, si.Timestamp.DateTime.Minute);
        Assert.Equal(info.Comment, si.Comment);
        Assert.Equal(info.Position?.Coordinates, si.Position?.Coordinates);
    }
}
