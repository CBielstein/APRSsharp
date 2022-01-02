namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using AprsSharp.Parsers.Aprs.Extensions;

    /// <summary>
    /// Represents an info field for packet type <see cref="PacketType.Status"/>.
    /// </summary>
    public class StatusInfo : InfoField
    {
        /// <summary>
        /// The list of characters which may not be used in a comment on this type.
        /// </summary>
        private static readonly char[] CommentDisallowedChars = { '|', '~' };

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusInfo"/> class.
        /// </summary>
        /// <param name="encodedInfoField">A string encoding of a <see cref="StatusInfo"/>.</param>
        public StatusInfo(string encodedInfoField)
        {
            if (string.IsNullOrWhiteSpace(encodedInfoField))
            {
                throw new ArgumentNullException(nameof(encodedInfoField));
            }

            Type = GetPacketType(encodedInfoField);

            if (Type != PacketType.Status)
            {
                throw new ArgumentException($"Packet encoding not of type {nameof(PacketType.Status)}. Type was {Type}", nameof(encodedInfoField));
            }

            Match match = Regex.Match(encodedInfoField, RegexStrings.StatusWithMaidenheadAndComment);
            if (match.Success)
            {
                match.AssertSuccess(PacketType.Status, nameof(encodedInfoField));

                Position = new Position();
                Position.DecodeMaidenhead(match.Groups[1].Value);

                if (match.Groups[4].Success)
                {
                    Comment = match.Groups[4].Value;
                }
            }
            else
            {
                // TODO Issue #88
                throw new NotImplementedException("Status report without maidenhead not yet implemented.Tracked by issue #88.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusInfo"/> class.
        /// </summary>
        /// <param name="position">A position, which will be encoded as a maidenhead gridsquare locator.</param>
        /// <param name="comment">An optional comment.</param>
        public StatusInfo(Position position, string? comment)
            : this(null, position, comment)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusInfo"/> class.
        /// </summary>
        /// <param name="timestamp">An optional timestamp.</param>
        /// <param name="comment">An optional comment.</param>
        public StatusInfo(Timestamp timestamp, string? comment)
            : this(timestamp, null, comment)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusInfo"/> class.
        /// Consolidates logic from the other constructors.
        /// </summary>
        /// <param name="timestamp">An optional timestamp.</param>
        /// <param name="position">A position, which will be encoded as a maidenhead gridsquare locator.</param>
        /// <param name="comment">An optional comment.</param>
        private StatusInfo(Timestamp? timestamp, Position? position, string? comment)
        {
            Type = PacketType.Status;

            if (position != null && timestamp != null)
            {
                throw new ArgumentException($"{nameof(timestamp)} may not be specified if a position is given.");
            }

            if (comment != null)
            {
                // Validate lengths
                if (position != null && comment.Length > 53)
                {
                    // Delimiting space + 53 characters = 54.
                    // We will add the space during encode, so only 53 here.
                    throw new ArgumentException(
                        $"With a position specified, comment may be at most 53 characters. Given comment had length {comment.Length}",
                        nameof(comment));
                }
                else if (timestamp != null && comment.Length > 55)
                {
                    throw new ArgumentException(
                        $"With a timestamp, comment may be at most 55 characters. Given comment had length {comment.Length}",
                        nameof(comment));
                }
                else if (comment.Length > 62)
                {
                    throw new ArgumentException(
                        $"Without timestamp or or position, comment may be at most 62 characters. Given comment had length {comment.Length}",
                        nameof(comment));
                }

                // TODO Issue #90: Share this logic across all packet types with a comment.
                // Validate no disallowed characters were used
                if (comment.IndexOfAny(CommentDisallowedChars) != -1)
                {
                    throw new ArgumentException($"Comment may not include `|` or `~` but was given: {comment}", nameof(comment));
                }
            }

            Timestamp = timestamp;
            Position = position;
            Comment = comment;
        }

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
        public Position? Position { get; }

        /// <inheritdoc/>
        public override string Encode()
        {
            StringBuilder encoded = new StringBuilder();

            if (Position != null)
            {
                if (Timestamp != null)
                {
                    throw new ArgumentException($"{nameof(Timestamp)} may not be specified if a position is given.");
                }

                encoded.Append(Type.ToChar());
                encoded.Append(Position.EncodeGridsquare(6, true));

                if (!string.IsNullOrEmpty(Comment))
                {
                    encoded.Append($" {Comment}");
                }
            }
            else
            {
                // TODO Issue #88
                throw new NotImplementedException("Status report without maidenhead not yet implemented.Tracked by issue #88.");
            }

            return encoded.ToString();
        }
    }
}
