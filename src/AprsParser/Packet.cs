﻿namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

            ReceivedTime = DateTime.UtcNow;

            // Attempt to decode TNC2 format
            Match match = Regex.Match(encodedPacket, RegexStrings.Tnc2Packet);
            if (match.Success)
            {
                match.AssertSuccess("Full TNC2 Packet", nameof(encodedPacket));
                Sender = match.Groups[1].Value;
                Path = match.Groups[2].Value.Split(',');
                InfoField = InfoField.FromString(match.Groups[3].Value);
                return;
            }

            // Next attempt to decode AX.25 format
            var packetBytes = Encoding.UTF8.GetBytes(encodedPacket);
            if (packetBytes.First() == (byte)Ax25Control.FLAG && packetBytes.Last() == (byte)Ax25Control.FLAG)
            {
                Sender = Packet.GetCallsignFromAx25(packetBytes, 0) ?? throw new ArgumentException("Missing sender");
                Destination = Packet.GetCallsignFromAx25(packetBytes, 1) ?? throw new ArgumentException("Missing destination");
                Path = new List<string>();

                for (var i = 2; i < 10; ++i)
                {
                    var pathEntry = Packet.GetCallsignFromAx25(packetBytes, i);
                    if (pathEntry == null)
                    {
                        break;
                    }

                    Path.Add(pathEntry);
                }

                var infoBytes = packetBytes.Skip(1 + ((Path.Count + 2) * 7) + 2).SkipLast(3);
                InfoField = InfoField.FromString(Encoding.UTF8.GetString(infoBytes.ToArray()));
                return;
            }

            throw new ArgumentException("Packet does not appear to be in supported format");
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
        /// Specifies different formats for a <see cref="Packet"/>.
        /// </summary>
        public enum Format
        {
            /// <summary>
            /// The TNC2 format.
            /// </summary>
            TNC2,

            /// <summary>
            /// The AX.25 packet format.
            /// </summary>
            AX25,
        }

        /// <summary>
        /// Special values used in <see cref="Format.AX25"/> encoding.
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
        /// Encodes an APRS packet to a string.
        /// </summary>
        /// <param name="format">The format to use for encoding.</param>
        /// <returns>String representation of the packet.</returns>
        public string Encode(Format format)
        {
            switch (format)
            {
                case Format.TNC2:
                    return $"{Sender}>{string.Join(',', Path)}:{InfoField.Encode()}";

                case Format.AX25:
                    throw new NotImplementedException($"Encoding not implemented for {nameof(Format.AX25)}");

                default:
                    throw new ArgumentException("Unsupported encoding format.", nameof(format));
            }
        }

        private static string? GetCallsignFromAx25(IEnumerable<byte> encodedPacket, int callsignNumber)
        {
            int callsignStart = 1 + (callsignNumber * 7);

            if (callsignStart + 7 >= encodedPacket.Count() ||
                (encodedPacket.ElementAt(callsignStart) == (byte)Ax25Control.UI_FRAME &&
                    encodedPacket.ElementAt(callsignStart + 1) == (byte)Ax25Control.NO_LAYER_THREE_PROTOCOL))
                {
                    return null;
                }

            return $"{Encoding.UTF8.GetString(encodedPacket.Skip(callsignStart).Take(6).ToArray())}-{encodedPacket.ElementAt(callsignStart + 6)}";
        }
    }
}
