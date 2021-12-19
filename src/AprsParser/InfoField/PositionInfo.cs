namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using AprsSharp.Parsers.Aprs.Extensions;

    /// <summary>
    /// Represents an APRS packet of the following types:
    ///     * <see cref="PacketType.PositionWithoutTimestampNoMessaging"/>
    ///     * <see cref="PacketType.PositionWithTimestampNoMessaging"/>
    ///     * <see cref="PacketType.PositionWithoutTimestampWithMessaging"/>
    ///     * <see cref="PacketType.PositionWithTimestampWithMessaging"/>.
    /// </summary>
    public class PositionInfo : InfoField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionInfo"/> class.
        /// </summary>
        /// <param name="encodedPacket">A string encoding of a <see cref="PositionInfo"/>.</param>
        public PositionInfo(string encodedPacket)
        {
            if (string.IsNullOrWhiteSpace(encodedPacket))
            {
                throw new ArgumentNullException(nameof(encodedPacket));
            }

            Type = GetPacketType(encodedPacket);

            if (Type == PacketType.PositionWithoutTimestampNoMessaging)
            {
                HasMessaging = false;
                Match match = Regex.Match(encodedPacket, RegexStrings.PositionWithoutTimestamp);
                match.AssertSuccess(PacketType.PositionWithoutTimestampNoMessaging, nameof(encodedPacket));

                Position = new Position(match.Groups[1].Value);

                if (match.Groups[6].Success)
                {
                    Comment = match.Groups[6].Value;
                }
            }
            else if (Type == PacketType.PositionWithoutTimestampWithMessaging)
            {
                HasMessaging = true;
                throw new NotImplementedException("Decoding not implemented for position without timestamp (with APRS messaging)");
            }
            else if (Type == PacketType.PositionWithTimestampNoMessaging || Type == PacketType.PositionWithTimestampWithMessaging)
            {
                HasMessaging = Type == PacketType.PositionWithTimestampWithMessaging;

                Match match = Regex.Match(encodedPacket, RegexStrings.PositionWithTimestamp);
                match.AssertSuccess(
                    HasMessaging ?
                        PacketType.PositionWithTimestampWithMessaging :
                        PacketType.PositionWithTimestampNoMessaging,
                    nameof(encodedPacket));

                Timestamp = new Timestamp(match.Groups[2].Value);

                Position = new Position(match.Groups[3].Value);

                if (match.Groups[8].Success)
                {
                    Comment = match.Groups[8].Value;
                }
            }
            else
            {
                throw new ArgumentException($"Packet encoding not one of the position types. Type was {Type}", nameof(encodedPacket));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionInfo"/> class.
        /// </summary>
        /// <param name="position"><see cref="Position"/> for this packet.</param>
        /// <param name="hasMessaging">True if the sender supports messaging.</param>
        /// <param name="timestamp">Optional <see cref="Timestamp"/> for this packet.</param>
        /// <param name="comment">Optional comment for this packet.</param>
        public PositionInfo(Position position, bool hasMessaging, Timestamp? timestamp, string? comment)
        {
            Position = position;
            HasMessaging = hasMessaging;
            Timestamp = timestamp;
            Comment = comment;

            if (Timestamp == null)
            {
                Type = HasMessaging ? PacketType.PositionWithoutTimestampWithMessaging
                    : PacketType.PositionWithoutTimestampNoMessaging;
            }
            else
            {
                Type = HasMessaging ? PacketType.PositionWithTimestampWithMessaging
                    : PacketType.PositionWithTimestampNoMessaging;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the sender of the packet supports messaging.
        /// </summary>
        public bool HasMessaging { get; }

        /// <summary>
        /// Gets the packet comment.
        /// </summary>
        public string? Comment { get; }

        /// <summary>
        /// Gets the time at which the message was sent.
        /// </summary>
        public Timestamp? Timestamp { get; }

        /// <summary>
        /// Gets the position from which the message was sent.
        /// </summary>
        public Position Position { get; }

        /// <inheritdoc/>
        public override string ToString() => ToString(TimestampType.DHMz);

        /// <summary>
        /// Encodes an APRS packet to a string.
        /// </summary>
        /// <param name="timeType">The <see cref="TimestampType"/> to use for timestamp encoding.</param>
        /// <returns>String representation of the packet.</returns>
        public string ToString(TimestampType timeType)
        {
            if (Position == null)
            {
                throw new ArgumentException($"Position cannot be null when encoding with type ${Type}");
            }

            StringBuilder encoded = new StringBuilder();

            encoded.Append(GetTypeChar(Type));

            if (Type == PacketType.PositionWithTimestampWithMessaging ||
                Type == PacketType.PositionWithTimestampNoMessaging)
            {
                if (Timestamp == null)
                {
                    throw new ArgumentException($"Timestamp cannot be null when encoding with type ${Type}");
                }

                encoded.Append(Timestamp.Encode(timeType));
            }

            encoded.Append(Position.Encode());
            encoded.Append(Comment);

            return encoded.ToString();
        }
    }
}
