namespace AprsSharp.AprsParser;

using System;

/// <summary>
/// Represents an info field that is not supported by APRS#.
/// </summary>
public class UnsupportedInfo : InfoField
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedInfo"/> class.
    /// </summary>
    /// <param name="encodedInfoField">A string encoding of an <see cref="InfoField"/>.</param>
    public UnsupportedInfo(string encodedInfoField)
        : base(encodedInfoField)
    {
        Content = encodedInfoField;
    }

    /// <summary>
    /// Gets the <see cref="InfoField"/> content.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Not supported. <see cref="UnsupportedInfo"/> is to help fill the gap on receive
    /// and should not be used for transmissions. We do not want to send non-standard packets
    /// and crowd the APRS airwaves. If you'd like to transmit a packet type that is not yet
    /// supported in APRS#, consider opening/commenting on an issue or sending a PR to
    /// implement the type.
    /// </summary>
    /// <throws><see cref="NotSupportedException"/>.</throws>
    /// <returns>Nothing, not supported.</returns>
    public override string Encode() => throw new NotSupportedException($"{nameof(UnsupportedInfo)} should not be used to encode packets for transmission. Please use a supported type.");
}
