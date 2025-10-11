namespace AprsSharp.AprsParser;

using System;

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
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override string Encode()
    {
        throw new NotImplementedException();
    }
}
