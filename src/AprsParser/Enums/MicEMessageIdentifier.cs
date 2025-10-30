namespace AprsSharp.AprsParser;

/// <summary>
/// Defines the message identifiers for encoding a <see cref="MicEMessageType"/>
/// in a Mic-E packet.
/// </summary>
internal enum MicEMessageIdentifier
{
    /// <summary>
    /// 0
    /// </summary>
    Zero,

    /// <summary>
    /// 1 (Custom)
    /// </summary>
    Custom1,

    /// <summary>
    /// 1 (Standard)
    /// </summary>
    Standard1,
}
