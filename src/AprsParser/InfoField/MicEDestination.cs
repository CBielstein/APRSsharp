namespace AprsSharp.AprsParser;

using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        if (micEBytes[3..].Any(b => CharInRange(b, 'A', 'K')))
        {
            throw new ArgumentException("Mic-E encoding does not use A-K in bytes 4-6.", nameof(encodedString));
        }

        Quadrant = DecodeQuadrant(micEBytes[3], micEBytes[5]);
        LongitudeOffset = DecodeLongitudeOffset(micEBytes[4]);
        Message = DecodeMessage(micEBytes[..3]);
        Latitude = DecodeLatitude(micEBytes[..6]);
    }

    /// <summary>
    /// Gets the latitutde that was encoded.
    /// </summary>
    public double Latitude { get; }

    /// <summary>
    /// Gets the message type.
    /// </summary>
    public MicEMessageType Message { get; }

    /// <summary>
    /// Gets the goegraphical quadrant of coordinates.
    /// </summary>
    public GeoQuadrant Quadrant { get; }

    /// <summary>
    /// Gets the offset to apply to longitude coordinates encoded in the info field.
    /// </summary>
    public uint LongitudeOffset { get; }

    /// <summary>
    /// Gets the digipath.
    /// </summary>
    public DigipeaterPaths Digipath { get; }

    /// <summary>
    /// Decodes the incidator bytes to get the <see cref="MicEMessageType"/>
    /// from the encoded packet.
    /// </summary>
    /// <param name="bytes">The relevant characters to decode for a message type.</param>
    /// <returns>An enum value representing the message encoded in the packet.</returns>
    private static MicEMessageType DecodeMessage(string bytes)
    {
        if (!Regex.IsMatch(bytes, @"^[0-9A-Z]{3}$"))
        {
            throw new ArgumentException($"Characters must be 0-9 or A-Z and have length of 3. Was given {bytes}", nameof(bytes));
        }

        // If the message contained one or more "standard" indicator
        bool standard = false;

        // If the message contained one or more "custom" indicator
        bool custom = false;

        var indicators = bytes.Select<char, byte>(b =>
        {
            if (b <= '9' || b == 'L')
            {
                return 0;
            }
            else if (b <= 'K')
            {
                custom = true;
                return 1;
            }
            else
            {
                standard = true;
                return 1;
            }
        }).ToArray();

        if (standard && custom)
        {
            // Conflicting indicators, this is unknown type
            return MicEMessageType.Unknown;
        }

        var indicatorByte =
            indicators[0] << 2 |
            indicators[1] << 1 |
            indicators[2] << 0;

        return indicatorByte switch
        {
            7 => standard ? MicEMessageType.OffDuty : MicEMessageType.Custom0,
            6 => standard ? MicEMessageType.EnRoute : MicEMessageType.Custom1,
            5 => standard ? MicEMessageType.InService : MicEMessageType.Custom2,
            4 => standard ? MicEMessageType.Returning : MicEMessageType.Custom3,
            3 => standard ? MicEMessageType.Committed : MicEMessageType.Custom4,
            2 => standard ? MicEMessageType.Special : MicEMessageType.Custom5,
            1 => standard ? MicEMessageType.Priority : MicEMessageType.Custom6,
            0 => MicEMessageType.Emergency,
            _ => throw new ArgumentException("Unsupported message value."),
        };
    }

    /// <summary>
    /// Decodes the quadrant of the message, based on given charaters.
    /// </summary>
    /// <param name="northSouth">The north/south indicator char.</param>
    /// <param name="eastWest">The east/west indicator char.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if not a valid indicator value.</exception>
    /// <returns>A <see cref="GeoQuadrant"/> represented by the given indicators.</returns>
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
    /// <exception cref="ArgumentOutOfRangeException">Thrown if not a valid indicator value.</exception>
    private static void ValidateHemisphereIndicator(char indicator)
    {
        if (!CharInRange(indicator, 'L', 'Z') && !CharInRange(indicator, '0', '9'))
        {
            throw new ArgumentOutOfRangeException(nameof(indicator), "Hemisphere (e.g. north/south, east/west) indicator needs to be in in ranges [0-9] or [L-Z]");
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

    /// <summary>
    /// Decodes the longitude offset.
    /// </summary>
    /// <param name="indicator">The character indicating a value.</param>
    /// <returns>Either 0 or 100, depending on the value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if not a valid indicator value.</exception>
    private static uint DecodeLongitudeOffset(char indicator)
    {
        if (!CharInRange(indicator, '0', '9') && !CharInRange(indicator, 'L', 'Z'))
        {
            throw new ArgumentOutOfRangeException(nameof(indicator), "Longitude offset indicator must not be in range A-K");
        }

        return indicator < 'P' ? 0u : 100u;
    }

    /// <summary>
    /// Decodes the latitude from the encoded mic-e bytes.
    /// </summary>
    /// <param name="bytes">Encoded mic-e destination string.</param>
    /// <returns>Decoded latitude.</returns>
    private static double DecodeLatitude(string bytes)
    {
        if (!Regex.IsMatch(bytes, @"^[0-9A-Z]{6}$"))
        {
            throw new ArgumentException($"Bytes for latitude decode must be 6 bytes long and in [0,9] or [A,Z]. Was given {bytes}", nameof(bytes));
        }

        var numbers = bytes.Select(b =>
        {
            if (b == 'K' || b == 'L' || b == 'Z')
            {
                // Space
                // TODO: Veify that it is okay to replace space with 0.
                // If not, what _should_ it be? Perhaps it's about ambiguity?
                return 0;
            }
            else if (b <= '9')
            {
                return b - '0';
            }
            else if (b <= 'J')
            {
                return b - 'J';
            }
            else
            {
                return b - 'P';
            }
        }).ToArray();

        int degrees = (numbers[0] * 10) + numbers[1];
        double minutes = (numbers[2] * 10) + numbers[3] + (numbers[4] / 10.0) + (numbers[5] / 100.0);

        return degrees + (minutes / 60.0);
    }
}
