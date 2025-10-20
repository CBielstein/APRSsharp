namespace AprsSharp.AprsParser;

using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Encodes and decodes the information in the destination field
/// of a Mic-E message format.
/// </summary>
public class MicEDestination
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MicEDestination"/> class
    /// by decoding the encoded string of a Mic-E-encoded destination field.
    /// </summary>
    /// <param name="encodedString">A string encoding of the destination field of a Mic-E packet.</param>
    public MicEDestination(string encodedString)
    {
        Match match = Regex.Match(encodedString, RegexStrings.CallsignWithOptionalSsid);
        if (!match.Success || match.Groups[2] == null)
        {
            throw new ArgumentException("Encoded destination should be a 6-char callsign plus SSID (e.g. N0CALL-0)", nameof(encodedString));
        }

        // Convert from SSID to path
        Digipath = (DigipeaterPaths)double.Parse(match.Groups[2].Value);

        string micEBytes = match.Groups[0].Value;

        // Some sanity checking against the encoding rules
        if (micEBytes[3..].Any(b => b >= 'A' && b <= 'K'))
        {
            throw new ArgumentException("Mic-E encoding does not use A-K in bytes 4-6.", nameof(encodedString));
        }

        Quadrant = DecodeQuadrant(micEBytes[3], micEBytes[5]);
    }

    /// <summary>
    /// Gets the latitutde that was encoded.
    /// </summary>
    public double Latitude { get; }

    /// <summary>
    /// Gets the message type.
    /// </summary>
    public MicEMessageIdentifier Message { get; }

    /// <summary>
    /// Gets the goegraphical quadrant of coordinates.
    /// </summary>
    public GeoQuadrant Quadrant { get; }

    /// <summary>
    /// Gets the offset to apply to longitude coordinates encoded in the info field.
    /// </summary>
    public double LongitudeOffset { get; }

    /// <summary>
    /// Gets the digipath.
    /// </summary>
    public DigipeaterPaths Digipath { get; }

    private static GeoQuadrant DecodeQuadrant(char northSouth, char eastWest)
    {
        ValidateHemisphereIndicator(northSouth);
        ValidateHemisphereIndicator(eastWest);

        bool north = northSouth > 'L';
        bool east = eastWest <= 'L';

        if (north && east)
        {
            return GeoQuadrant.Northeast;
        }
        else if (north && !east)
        {
            return GeoQuadrant.Northwest;
        }
        else if (!north && east)
        {
            return GeoQuadrant.Southeast;
        }
        else
        {
            return GeoQuadrant.Southwest;
        }
    }

    /// <summary>
    /// Validates if the hemsiphere indicators (north/sound vs east/west)
    /// are in valid ranges.
    /// </summary>
    /// <param name="indicator">Indicator to check.</param>
    /// <exception cref="ArgumentException">Thrown if not a valid indicator value.</exception>
    private static void ValidateHemisphereIndicator(char indicator)
    {
        if (!CharInRange(indicator, 'L', 'Z') && !CharInRange(indicator, '0', '9'))
        {
            throw new ArgumentException("Hemisphere (e.g. north/south, east/west) indicator needs to be in in ranges [0-9] or [L-Z]");
        }
    }

    /// <summary>
    /// Checks if a `check` variable is in an inclusive range of characters
    /// in ASCII encodings.
    /// </summary>
    /// <param name="check">The value to check.</param>
    /// <param name="a">The starting value (inclusive).</param>
    /// <param name="b">The closing vlaue (inclusive).</param>
    /// <returns>True, if check is in range [a,b]. Else, false.</returns>
    private static bool CharInRange(char check, char a, char b)
    {
        if (b < a)
        {
            throw new ArgumentException("a must be less than or equal to b.");
        }

        return a <= check && check <= b;
    }
}