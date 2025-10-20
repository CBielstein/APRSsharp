namespace AprsSharp.AprsParser;

using System;
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
}