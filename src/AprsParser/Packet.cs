namespace AprsSharp.Parsers.Aprs
{
    using System;
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

            InfoField = InfoField.FromString(encodedPacket.Split(':', 2).Last());
        }

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