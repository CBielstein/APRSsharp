namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

            // TODO Issue #79: Actually parse the full fields in the APRS/TNC2 string.
            InfoField = InfoField.FromString(encodedPacket.Split(':', 2).Last());
        }

        /// <summary>
        /// Gets the sender's callsign.
        /// </summary>
        public string Sender { get; }

        /// <summary>
        /// Gets the APRS path (UNPROTO path) of the packet.
        /// </summary>
        public IList<string> Path { get; }

        /// <summary>
        /// Gets the time this packet was received.
        /// </summary>
        public DateTime ReceivedTime { get; }

        /// <summary>
        /// Gets the APRS information field for this packet.
        /// </summary>
        public InfoField InfoField { get; }

        /// <summary>
        /// Encodes an APRS packet to a string.
        /// </summary>
        /// <returns>String representation of the packet.</returns>
        public virtual string Encode()
        {
            throw new NotImplementedException();
        }
    }
}
