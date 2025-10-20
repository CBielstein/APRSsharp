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
        Assert.Equal(MicEMessageIdentifier.Returning, dest.Message);
        Assert.Equal(33.427333, dest.Latitude);
        Assert.Equal(33.427333, dest.Latitude);
        Assert.Equal(0, dest.LongitudeOffset);
        Assert.Equal(GeoQuadrant.Northwest, dest.Quadrant);
        Assert.Equal(DigipeaterPaths.WIDE11, dest.Digipath);
    }
}
