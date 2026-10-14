namespace AprsSharp.AprsParser;

using System;
using System.Text;
using System.Linq;
using GeoCoordinatePortable;

/// <summary>
/// Represents an info field for Mic-E encoded packets.
/// This includes some information that was technically encoded in the destination field,
/// but will be saved here to match the format of other packets.
/// </summary>
public class MicEInfo : InfoField
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MicEInfo"/> class
    /// by decoding the encoded destination and information fields.
    /// </summary>
    /// <param name="destinationField">A string encoding of the information field of a Mic-E packet.</param>
    /// <param name="encodedInfoField">Byte encoding of the information field of a Mic-E packet.</param>
    public MicEInfo(string destinationField, byte[] encodedInfoField)
        : base(encodedInfoField)
    {
        if (encodedInfoField.Length < 9)
        {
            throw new ArgumentException("Mic-E info field should be at least 9 bytes long.", nameof(encodedInfoField));
        }

        MicEDestination decodedDestination = new MicEDestination(destinationField);

        double latitude = decodedDestination.Latitude;
        double longitude = 0 + decodedDestination.LongitudeOffset;
        double? altitude = null;

        Message = decodedDestination.Message;

        if (encodedInfoField.Length > 9 && (encodedInfoField[9] == '‘' || encodedInfoField[9] == '\'' || encodedInfoField[9] == 0x1d))
        {
            // This is telemetry data
        }
        else
        {
            // This is status data

            // check if this is a standard position info field encoding wut
            try
            {
                PositionInfo positionInfo = new PositionInfo(Encoding.ASCII.GetString(encodedInfoField));
                Position = positionInfo.Position;
            }
            catch
            {
            }

            // if it isn't...
            // Check for maidenhead locator
            // if (encodedInfoField.Contains("/G"))
            // {
            //     Position = new Position.DecodeMaidenhead();
            // }

            // Check for altitude
            int i = Array.IndexOf(encodedInfoField, "}");

            // There needs to be 3 characters before the indicator: xxx}
            if (i >= 3)
            {
                // Subtract 33 from each byte from the encoding...
                byte[] altitudeBytes = encodedInfoField[(i-3)..(i+1)].Select(b => (byte)(b - 33)).ToArray();

                // Altitude in meters above 10km below sea level
                int altitudeAboveDatum = Position.DecodeBase91(altitudeBytes);

                altitude = altitudeAboveDatum - 10000;
            }
        }

        GeoCoordinate coords = altitude is not null ?
            new GeoCoordinate(latitude, longitude, altitude.Value) :
            new GeoCoordinate(latitude, longitude);

        // Position set by an encoded PositionInfo takes priority over the mic-e position
        Position ??= new Position(coords);
    }

    /// <summary>
    /// Gets the position from which the message was sent.
    /// </summary>
    public Position Position { get; }

    /// <summary>
    /// Gets the mic-e message type.
    /// </summary>
    public MicEMessageType Message { get; }

    /// <inheritdoc/>
    public override string Encode()
    {
        throw new NotImplementedException();
    }
}
