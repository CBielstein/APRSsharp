namespace AprsSharp.AprsParser;

using System;
using GeoCoordinatePortable;

/// <summary>
/// Represents an info field for Mic-E encoded packets.
/// This includes some information that was technically encoded in the destination field,
/// but will be saved here to match the format of other packets.
/// </summary>
public class MicEInfo : InfoField
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MicEInfo"/> class.
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

        double latitude = 0; // TODO
        double longitude = 0; // TODO
        GeoCoordinate coords = new GeoCoordinate(latitude, longitude); // TODO: Altitude
        Position = new Position()

        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the position from which the message was sent.
    /// </summary>
    public Position Position { get; }

    /// <inheritdoc/>
    public override string Encode()
    {
        throw new NotImplementedException();
    }
}
