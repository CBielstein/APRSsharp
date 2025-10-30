namespace AprsSharpUnitTests.AprsParser;

using System;
using System.Text;
using AprsSharp.AprsParser;
using Xunit;

/// <summary>
/// Tests ability of the <see cref="MicEDestination"/> to encode and decode mic-e messages.
/// </summary>
public class MicEDestinationTests
{
    /// <summary>
    /// Validates the result of the example given in APRS 1.01.
    /// </summary>
    [Fact]
    public void DecodeFromAprs101()
    {
        // APRS 1.01 specifies "S32U6T", I added "-1" so that it would be representative of the full encoding.
        MicEDestination dest = new MicEDestination("S32U6T-1");

        // should be 33.427333, but added an extra 3 and allowing for floating point stuff
        Assert.Equal(33.4273333, dest.Latitude, 0.0000001);
        Assert.Equal(0u, dest.LongitudeOffset);
        Assert.Equal(MicEMessageType.Returning, dest.Message);
        Assert.Equal(GeoQuadrant.Northwest, dest.Quadrant);
        Assert.Equal(DigipeaterPaths.WIDE11, dest.Digipath);
    }

    /// <summary>
    /// Mic-E encoding doesn't allow chars A-K in the 4th, 5th, or 6th location.
    /// Validate that an exception is thrown.
    /// </summary>
    /// <param name="encodedBytes">The encoded string to check.</param>
    [Theory]
    [InlineData("AAAAAA-0")]
    [InlineData("000A00-0")]
    [InlineData("000B00-0")]
    [InlineData("000C00-0")]
    [InlineData("000D00-0")]
    [InlineData("000E00-0")]
    [InlineData("0000F0-0")]
    [InlineData("0000G0-0")]
    [InlineData("0000H0-0")]
    [InlineData("00000I-0")]
    [InlineData("00000J-0")]
    [InlineData("00000K-0")]
    public void CharatersOutOfPlace(string encodedBytes)
        => Assert.Throws<ArgumentException>(() => new MicEDestination(encodedBytes));
}
