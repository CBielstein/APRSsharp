using System;

namespace APaRSer
{
    /// <summary>
    /// A representation of an APRS Packet.
    /// Does decoding of an APRS packet as a string.
    /// </summary>
    public class APRSPacket
    {
        /// <summary>
        /// Constructs a new APRSPacket object by decoding the encodedPacket string.
        /// Really just a shortcut for calling the Decode function.
        /// </summary>
        /// <param name="encodedPacket">A string encoding of an APRS packet</param>
        public APRSPacket(string encodedPacket)
        {
            Decode(encodedPacket);
        }

        /// <summary>
        /// Takes a string encoding of an APRS packet, decodes it, and saves it in to this object.
        /// </summary>
        /// <param name="encodedPacket">A string encoding of an APRS packet</param>
        public void Decode(string encodedPacket)
        {
            throw new NotImplementedException("APRSPacket.Decode(string)");
        }
    }
}
