namespace AprsSharpUnitTests.Parsers.Aprs;

using System;
using System.Linq;
using AprsSharp.Parsers.Aprs;
using Xunit;

/// <summary>
/// Tests code in the <see cref="UnsupportedInfo"/> class for displaying unsupported/unknown formats.
/// </summary>
public class UnsupportedInfoUnitTests
{
    /// <summary>
    /// Verifies a (currently) unsupported packet type results in an <see cref="UnsupportedInfo"/>.
    /// NOTE: This will have to be changed when that type is actually supported.
    ///     This is currently a packet of "Item Report Format — with Lat/Long position".
    /// </summary>
    [Fact]
    public void TestUnsupportedType()
    {
        // "Item Report Format — with Lat/Long position" example from APRS 1.01 Spec
        string encoded = "N0CALL>WIDE2-2:)AID #2!4903.50N/07201.75WA";
        Packet p = new Packet(encoded);

        Assert.Equal("N0CALL", p.Sender);
        Assert.Equal("WIDE2-2", p.Path.Single());
        Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1));
        Assert.Equal(PacketType.Item, p.InfoField.Type);
        Assert.IsType<UnsupportedInfo>(p.InfoField);

        var ui = p.InfoField as UnsupportedInfo;
        Assert.Equal(")AID #2!4903.50N/07201.75WA", ui!.Content);
    }

    /// <summary>
    /// Verifies an invalid packet type results in an <see cref="UnsupportedInfo"/>.
    /// </summary>
    [Fact]
    public void TestInvalidDataType()
    {
        // "Invalid Data / Test Data Format" example from APRS 1.01 Spec
        string encoded = "N0CALL>WIDE2-2:,191146,V,4214.2466,N,07303.5181,W,417.238,114.5,091099,14.7,W/GPS FIX";
        Packet p = new Packet(encoded);

        Assert.Equal("N0CALL", p.Sender);
        Assert.Equal("WIDE2-2", p.Path.Single());
        Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1));
        Assert.Equal(PacketType.InvalidOrTestData, p.InfoField.Type);
        Assert.IsType<UnsupportedInfo>(p.InfoField);

        var ui = p.InfoField as UnsupportedInfo;
        Assert.Equal(",191146,V,4214.2466,N,07303.5181,W,417.238,114.5,091099,14.7,W/GPS FIX", ui!.Content);
    }

    /// <summary>
    /// Verifies that a completely unknown encoding results in an <see cref="UnsupportedInfo"/>.
    /// </summary>
    [Fact]
    public void TestUnknownEncoding()
    {
        string encoded = "N0CALL>WIDE2-2:EXAMPLE UNKNOWN ENCODING";
        Packet p = new Packet(encoded);

        Assert.Equal("N0CALL", p.Sender);
        Assert.Equal("WIDE2-2", p.Path.Single());
        Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1));
        Assert.Equal(PacketType.Unknown, p.InfoField.Type);
        Assert.IsType<UnsupportedInfo>(p.InfoField);

        var ui = p.InfoField as UnsupportedInfo;
        Assert.Equal("EXAMPLE UNKNOWN ENCODING", ui!.Content);
    }

    /// <summary>
    /// Verifies that trying to encode an <see cref="UnsupportedInfo"/> results in an exception.
    /// </summary>
    [Fact]
    public void EncodeUnknownThrows()
    {
        UnsupportedInfo ui = new UnsupportedInfo("Some data");
        Packet p = new Packet("N0CALL", Array.Empty<string>(), ui);
        Assert.Throws<NotSupportedException>(() => p.Encode());
    }
}
