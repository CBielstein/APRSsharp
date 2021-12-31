namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using AprsSharp.Parsers.Aprs.Extensions;

    /// <summary>
    /// Represents an info field for the following types of packet:
    ///     * <see cref="PacketType.Status"/>.
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
                    ValidateComment(Comment);
                }
            }
            else
            {
                throw new NotImplementedException("Status report without maidenhead not yet implemented.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusInfo"/> class.
        /// </summary>
        /// <param name="position">A position, which will be encoded as a maidenhead gridsquare locator.</param>
        /// <param name="comment">An optional comment.</param>
        public StatusInfo(Position position, string? comment)
        {
            if (comment != null)
            {
                ValidateComment(comment);
            }

            Position = position;
            Comment = comment;
            Timestamp = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusInfo"/> class.
        /// </summary>
        /// <param name="timestamp">An optional timestamp.</param>
        /// <param name="comment">An optional comment.</param>
        public StatusInfo(Timestamp timestamp, string? comment)
        {
            if (comment != null)
            {
                ValidateComment(comment);
            }

            Timestamp = timestamp;
            Comment = comment;
            Position = null;
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
                throw new NotImplementedException("Status report without maidenhead not yet implemented.");
            }

            return encoded.ToString();
        }

        /// <summary>
        /// Validates that a comment string does not contain disallowed characters.
        /// </summary>
        /// <param name="comment">A comment string to verify.</param>
        private static void ValidateComment(string comment)
        {
            if (comment.IndexOfAny(CommentDisallowedChars) != -1)
            {
                throw new ArgumentException($"Comment may not include `|` or `~` but was given: {comment}", nameof(comment));
            }
        }
    }
}
