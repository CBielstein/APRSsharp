namespace AprsSharp.AprsParser.Extensions;

/// <summary>
/// Extension methods for dealing with <see cref="PacketType"/> .
/// </summary>
public static class PacketTypeExtensions
{
    /// <summary>
    /// Determines if a <see cref="PacketType"/> is one of the Mic-E formats.
    /// </summary>
    /// <param name="type"><see cref="PacketType"/> to test.</param>
    /// <returns>True if one of the Mic-E format, else false.</returns>
    public static bool IsMicEType(this PacketType type)
        => type == PacketType.OldMicEData ||
            type == PacketType.CurrentMicEData ||
            type == PacketType.OldMicEDataCurrentTMD700 ||
            type == PacketType.CurrentMicEDataNotTMD700;
}
