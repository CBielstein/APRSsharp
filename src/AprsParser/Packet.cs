namespace AprsSharp.AprsParser
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using AprsSharp.AprsParser.Extensions;

    /// <summary>
    /// Represents a full APRS packet.
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="encodedPacket">The encoded APRS packet.</param>
        public Packet(byte[] encodedPacket)
        {
            if (encodedPacket == null || encodedPacket.Length == 0)
            {
                throw new ArgumentNullException(nameof(encodedPacket));
            }

            ReceivedTime = DateTime.UtcNow;

            // Attempt to decode TNC2 format
            Match match = Regex.Match(Encoding.ASCII.GetString(encodedPacket), RegexStrings.Tnc2Packet);
            if (match.Success)
            {
                match.AssertSuccess("Full TNC2 Packet", nameof(encodedPacket));
                Sender = match.Groups[1].Value;
                Path = match.Groups[2].Value.Split(',');
                InfoField = InfoField.FromString(match.Groups[3].Value);
                return;
            }

            // Next attempt to decode AX.25 format
            Destination = GetCallsignFromAx25(encodedPacket, 0, out _);
            Sender = GetCallsignFromAx25(encodedPacket, 1, out bool isFinalAddress);
            Path = new List<string>();

            for (var i = 2; !isFinalAddress && i < 10; ++i)
            {
                var pathEntry = GetCallsignFromAx25(encodedPacket, i, out isFinalAddress);
                Path.Add(pathEntry);
            }

            var infoBytes = encodedPacket.Skip(((Path.Count + 2) * 7) + 2);
            InfoField = InfoField.FromString(Encoding.ASCII.GetString(infoBytes.ToArray()));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="encodedPacket">A string representation of an encoded packet.</param>
        public Packet(string encodedPacket)
            : this(Encoding.ASCII.GetBytes(encodedPacket))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="sender">The callsign of the sender.</param>
        /// <param name="path">The digipath for the packet.</param>
        /// <param name="infoField">An <see cref="InfoField"/> or extending class to send.</param>
        public Packet(string sender, IList<string> path, InfoField infoField)
            : this(sender, null, path, infoField)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="sender">The callsign of the sender.</param>
        /// <param name="destination">The callsign of the destination.</param>
        /// <param name="path">The digipath for the packet.</param>
        /// <param name="infoField">An <see cref="InfoField"/> or extending class to send.</param>
        public Packet(string sender, string? destination, IList<string> path, InfoField infoField)
        {
            Sender = sender;
            Destination = destination;
            Path = path;
            InfoField = infoField;
            ReceivedTime = null;
        }

        /// <summary>
        /// Special values used in AX.25 encoding.
        /// </summary>
        private enum Ax25Control : byte
        {
            /// <summary>
            /// Flag value used to mark start and end of frames.
            /// </summary>
            FLAG = 0x7e,

            /// <summary>
            /// Control field value to denote UI-frame.
            /// </summary>
            UI_FRAME = 0x03,

            /// <summary>
            /// Protocol ID value to specify no layer three.
            /// </summary>
            NO_LAYER_THREE_PROTOCOL = 0xf0,
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
        /// Gets the destination callsign.
        /// </summary>
        public string? Destination { get; }

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

        /// <summary>
        /// Encodes an APRS packet as bytes in AX.25 format.
        /// </summary>
        /// <returns>Byte encoding of packet in AX.25 format.</returns>
        public byte[] EncodeAx25()
        {
            var encodedInfoField = InfoField.Encode();

            // Length
            // Sender address (7) + Destination address (7)
            // + Path (7*N)
            // + Control Field (1) + Protocol ID (1)
            // + Info field (N)
            var numBytes = 16 + (Path.Count * 7) + encodedInfoField.Length;
            var encodedBytes = new byte[numBytes];

            var offset = 0;

            EncodeCallsignBytes(Destination).CopyTo(encodedBytes, offset);
            offset += 7;

            EncodeCallsignBytes(Sender, Path.Count == 0).CopyTo(encodedBytes, offset);
            offset += 7;

            if (Path.Count > 8)
            {
                throw new ArgumentException("Path must not have more than 8 entries");
            }

            for (var i = 0; i < Path.Count; ++i)
            {
                EncodeCallsignBytes(Path[i], i == (Path.Count - 1)).CopyTo(encodedBytes, offset);
                offset += 7;
            }

            encodedBytes[offset] = (byte)Ax25Control.UI_FRAME;
            offset += 1;

            encodedBytes[offset] = (byte)Ax25Control.NO_LAYER_THREE_PROTOCOL;
            offset += 1;

            Encoding.ASCII.GetBytes(encodedInfoField).CopyTo(encodedBytes, offset);

            return encodedBytes;
        }

        /// <summary>
        /// Attempts to get a callsign from an encoded AX.25 packet.
        /// </summary>
        /// <param name="encodedPacket">The bytes of the encoded packet.</param>
        /// <param name="callsignNumber">The number of the callsign (in order in the packet encoding) to attempt to fetch.</param>
        /// <param name="isFinal">True if this is flagged as the final address in the frame.</param>
        /// <returns>A callsign string.</returns>
        private static string GetCallsignFromAx25(IEnumerable<byte> encodedPacket, int callsignNumber, out bool isFinal)
        {
            int callsignStart = callsignNumber * 7;

            var raw = encodedPacket.Skip(callsignStart).Take(6);
            var shifted = raw.Select(r => (byte)(r >> 1)).ToArray();
            var callsign = Encoding.ASCII.GetString(shifted).Trim();

            var ssid = encodedPacket.ElementAt(callsignStart + 6);

            isFinal = (ssid & 0b1) == 1;

            ssid >>= 1;
            ssid &= 0b1111;

            return ssid == 0x0 ? callsign : $"{callsign}-{ssid}";
        }

        /// <summary>
        /// Encodes a callsign in to the appropriate bytes for AX.25.
        /// This includes: 6 bytes of callsign (padded spaces left)
        /// 1 byte of SSID as a byte value (not ASCII numbers).
        /// If callsign is null, return all spaces and SSID 0.
        /// </summary>
        /// <param name="callsign">Callsign to encode.</param>
        /// <param name="isFinal">Boolean value indicating if this is the final callsign in the address list.
        ///     If so, a flag is set according to AX.25 spec.</param>
        /// <returns>Encoded bytes of callsign. Should always be 7 bytes.</returns>
        private static byte[] EncodeCallsignBytes(string? callsign, bool isFinal = false)
        {
            if (callsign == null)
            {
                return Encoding.ASCII.GetBytes(new string(' ', 6)).Append((byte)0).ToArray();
            }

            var matches = Regex.Match(callsign, RegexStrings.CallsignWithOptionalSsid);
            matches.AssertSuccess(nameof(RegexStrings.CallsignWithOptionalSsid), nameof(callsign));

            var call = matches.Groups[1].Value.PadRight(6, ' ');
            int ssid = int.TryParse(matches.Groups[3].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int inputSsid) ?
                   inputSsid :
                   0;

            if (ssid > 15 || ssid < 0)
            {
                throw new ArgumentException("SSID must be in range [0,15]", nameof(callsign));
            }

            var ssidByte = (byte)(0b1110000 | (ssid & 0b1111));
            ssidByte <<= 1;

            if (isFinal)
            {
                ssidByte |= 0x1;
            }

            var shiftedBytes = Encoding.ASCII.GetBytes(call).Select(raw => (byte)(raw << 1)).ToArray();
            return shiftedBytes.Append(ssidByte).ToArray();
        }
    }
}
