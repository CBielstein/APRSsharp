namespace AprsSharp.AprsParser;

using System;

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
        throw new NotImplementedException();
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