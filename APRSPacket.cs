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
            throw new NotImplementedException();
        }

        private void DecodeInformationField(string informationField)
        {
            if (informationField == null)
            {
                throw new ArgumentNullException();
            }
            else if (informationField.Length == 0)
            {
                throw new ArgumentException("Empty string argument");
            }

            char dataTypeIdentifier = informationField[0];
            
            switch (dataTypeIdentifier)
            {
                case (char)0x1c:
                    throw new NotImplementedException("handle currnet Mic-E Data (Rev 0 beta)");
                case (char)0x1d:
                    throw new NotImplementedException("handle old Mic-E Data (Rev 0 beta)");
                case '!':
                    throw new NotImplementedException("handle position without timestamp (no APRS messaging), or Ultimeter 2000 WX Station");
                case '#':
                    throw new NotImplementedException("handle Peet Bros U-II Weather Station");
                case '$':
                    throw new NotImplementedException("handle Raw GPS data or Ultimeter 2000");
                case '%':
                    throw new NotImplementedException("handle Agrelo DFJr / MicroFinder");
                case '&':
                    throw new NotImplementedException("handle Reserved - Map Feature");
                case '\'':
                    throw new NotImplementedException("handle Old Mic-E Data (but Current data for TM-D700");
                case ')':
                    throw new NotImplementedException("handle Item");
                case '*':
                    throw new NotImplementedException("handle Peet Bros U-II Weather Station");
                case '+':
                    throw new NotImplementedException("handle Reserved - Shelter data with time");
                case ',':
                    throw new NotImplementedException("handle Invalid data or test data");
                case '.':
                    throw new NotImplementedException("handle Reserved - Space weather");
                case '/':
                    throw new NotImplementedException("handle Position with timestamp (no APRS messaging)");
                case ':':
                    throw new NotImplementedException("handle Message");
                case ';':
                    throw new NotImplementedException("handle Object");
                case '<':
                    throw new NotImplementedException("handle Station capabilities");
                case '=':
                    throw new NotImplementedException("handle Position without timestamp (with APRS messaging)");
                case '>':
                    throw new NotImplementedException("handle Status");
                case '?':
                    throw new NotImplementedException("handle Query");
                case '@':
                    throw new NotImplementedException("handle Position with timestamp (with APRS messaging)");
                case 'T':
                    throw new NotImplementedException("handle Telemetry data");
                case '[':
                    throw new NotImplementedException("handle Maidenhead grid locator beacon (obsolete)");
                case '_':
                    throw new NotImplementedException("handle Weather report (without position)");
                case '`':
                    throw new NotImplementedException("handle Current Mic-E Data (not used in TM-D700");
                case '{':
                    throw new NotImplementedException("handle User-Defined APRS packet format");
                case '}':
                    throw new NotImplementedException("handle Third-party traffic");
                default:
                    throw new ArgumentException("Information field started with invalid character: " + dataTypeIdentifier);
            }

            throw new NotImplementedException("APRSPacket.DecodeInformationField");
        }
    }
}
