namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using AprsSharp.Parsers.Aprs.Extensions;

    /// <summary>
    /// Represents a full APRS packet.
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="encodedPacket">The string encoding of the APRS packet.</param>
        public Packet(string encodedPacket)
        {
            if (string.IsNullOrWhiteSpace(encodedPacket))
            {
                throw new ArgumentNullException(nameof(encodedPacket));
            }

            Match match = Regex.Match(encodedPacket, RegexStrings.Tnc2Packet);
            match.AssertSuccess("Full TNC2 Packet", nameof(encodedPacket));

            Sender = match.Groups[1].Value;
            Path = match.Groups[2].Value.Split(',');
            ReceivedTime = DateTime.UtcNow;
            InfoField = InfoField.FromString(match.Groups[3].Value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="sender">The callsign of the sender.</param>
        /// <param name="path">The digipath for the packet.</param>
        /// <param name="infoField">An <see cref="InfoField"/> or extending class to send.</param>
        public Packet(string sender, IList<string> path, InfoField infoField)
        {
            Sender = sender;
            Path = path;
            InfoField = infoField;
            ReceivedTime = null;
        }

        /// <summary>
        /// Gets the sender's callsign.
        /// </summary>
        public string Sender { get; }

        /// <summary>
        /// Gets the APRS digipath of the packet.
        /// </summary>
        public IList<string> Path { get; }

        /// <summary>
        /// Gets the time this packet was decoded.
        /// Null if this packet was created instead of decoded.
        /// </summary>
        public DateTime? ReceivedTime { get; }

        /// <summary>
        /// Gets the APRS information field for this packet.
        /// </summary>
        public InfoField InfoField { get; }

        /// <summary>
        /// Encodes an APRS packet as a string in TNC2 format.
        /// </summary>
        /// <returns>String of packet in TNC2 format.</returns>
        public string EncodeTnc2()
        {
            return $"{Sender}>{string.Join(',', Path)}:{InfoField.Encode()}";
        }
    }
}
