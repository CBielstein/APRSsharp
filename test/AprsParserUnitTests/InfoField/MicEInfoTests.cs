namespace AprsSharpUnitTests.AprsParser;

using System;
using System.Text;
using AprsSharp.AprsParser;
using Xunit;

/// <summary>
/// Tests ability of the <see cref="MicEInfo"/> to encode and decode mic-e messages.
/// </summary>
public class MicEInfoTests
{
    /// <summary>
    /// Validates that Mic-E packets with an info field less than 9 bytes
    /// results in an <see cref="ArgumentException"/> via the constructor.
    /// </summary>
    [Fact]
    public void InfoFieldMustBe9BytesConstructor()
    {
        string destination = "N0CALL-0";
        byte[] info = Encoding.ASCII.GetBytes("12345678");
        Assert.Throws<ArgumentException>(() => new MicEInfo(destination, info));
    }
}
