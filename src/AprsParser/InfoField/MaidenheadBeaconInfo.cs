namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using AprsSharp.Parsers.Aprs.Extensions;

    /// <summary>
    /// Represents an info field for packet type <see cref="PacketType.MaidenheadGridLocatorBeacon"/>.
    /// </summary>
    public class MaidenheadBeaconInfo : InfoField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaidenheadBeaconInfo"/> class.
        /// NOTE: This packet type is considere obsolete and is included for decoding legacy devices. Use a status report instead.
        /// </summary>
        /// <param name="encodedInfoField">A string encoding of a <see cref="StatusInfo"/>.</param>
        public MaidenheadBeaconInfo(string encodedInfoField)
        {
            if (string.IsNullOrWhiteSpace(encodedInfoField))
            {
                throw new ArgumentNullException(nameof(encodedInfoField));
            }

            Type = GetPacketType(encodedInfoField);

            if (Type != PacketType.MaidenheadGridLocatorBeacon)
            {
                throw new ArgumentException($"Packet encoding not of type {nameof(PacketType.MaidenheadGridLocatorBeacon)}. Type was {Type}", nameof(encodedInfoField));
            }

            Match match = Regex.Match(encodedInfoField, RegexStrings.MaidenheadGridLocatorBeacon);
            match.AssertSuccess(PacketType.Status, nameof(encodedInfoField));

            Position = new Position();
            Position.DecodeMaidenhead(match.Groups[1].Value);

            if (match.Groups[2].Success)
            {
                Comment = match.Groups[2].Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaidenheadBeaconInfo"/> class.
        /// </summary>
        /// <param name="position">A position, which will be encoded as a maidenhead gridsquare locator.</param>
        /// <param name="comment">An optional comment.</param>
        public MaidenheadBeaconInfo(Position position, string? comment)
        {
            Type = PacketType.MaidenheadGridLocatorBeacon;
            Comment = comment;
            Position = position ?? throw new ArgumentNullException(nameof(position));
        }

        /// <summary>
        /// Gets the packet comment.
        /// </summary>
        public string? Comment { get; }

        /// <summary>
        /// Gets the position from which the message was sent.
        /// </summary>
        public Position Position { get; }

        /// <inheritdoc/>
        public override string Encode()
        {
            StringBuilder encoded = new StringBuilder();

            encoded.Append($"[{Position.EncodeGridsquare(6, false)}]");

            if (!string.IsNullOrEmpty(Comment))
            {
                encoded.Append($" {Comment}");
            }

            return encoded.ToString();
        }
    }
}
