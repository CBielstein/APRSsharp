namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using AprsSharp.Parsers.Aprs.Extensions;

    /// <summary>
    /// A representation of an APRS Packet.
    /// Does decoding of an APRS packet as a string.
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// Maps a type-identifying character to a packet <see cref="PacketType"/>.
        /// </summary>
        private static readonly Dictionary<char, PacketType> DataTypeMap = new Dictionary<char, PacketType>()
        {
            { (char)0x1c, PacketType.CurrentMicEData },
            { (char)0x1d, PacketType.OldMicEData },
            { '!', PacketType.PositionWithoutTimestampNoMessaging },
            { '\"', PacketType.Unused },
            { '#', PacketType.PeetBrosUIIWeatherStation },
            { '$', PacketType.RawGPSData },
            { '%', PacketType.AgreloDFJrMicroFinder },
            { '&', PacketType.MapFeature },
            { '\'', PacketType.OldMicEDataCurrentTMD700 },
            { '(', PacketType.Unused },
            { ')', PacketType.Item },
            { '*', PacketType.PeetBrosUIIWeatherStation },
            { '+', PacketType.ShelterDataWithTime },
            { ',', PacketType.InvalidOrTestData },
            { '-', PacketType.Unused },
            { '.', PacketType.SpaceWeather },
            { '/', PacketType.PositionWithTimestampNoMessaging },
            { ':', PacketType.Message },
            { ';', PacketType.Object },
            { '<', PacketType.StationCapabilities },
            { '=', PacketType.PositionWithoutTimestampWithMessaging },
            { '>', PacketType.Status },
            { '?', PacketType.Query },
            { '@', PacketType.PositionWithTimestampWithMessaging },
            { 'T', PacketType.TelemetryData },
            { '[', PacketType.MaidenheadGridLocatorBeacon },
            { '\\', PacketType.Unused },
            { ']', PacketType.Unused },
            { '^', PacketType.Unused },
            { '_', PacketType.WeatherReport },
            { '`', PacketType.CurrentMicEDataNotTMD700 },
            { '{', PacketType.UserDefinedAPRSPacketFormat },
            { '}', PacketType.ThirdPartyTraffic },
            { 'A', PacketType.DoNotUse },
            { 'S', PacketType.DoNotUse },
            { 'U', PacketType.DoNotUse },
            { 'Z', PacketType.DoNotUse },
            { '0', PacketType.DoNotUse },
            { '9', PacketType.DoNotUse },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// If an encoded packet is provided, Decode is used to deocde in to this object.
        /// </summary>
        /// <param name="encodedPacket">A string encoding of an APRS packet to decode (optional).</param>
        public Packet(string? encodedPacket = null)
        {
            if (encodedPacket != null)
            {
                Decode(encodedPacket);
            }
        }

        /// <summary>
        /// Gets or sets the time at which the packet was sent.
        /// </summary>
        public Timestamp? Timestamp { get; set; } = null;

        /// <summary>
        /// Gets or sets the location from which the packet was sent.
        /// </summary>
        public Position? Position { get; set; } = null;

        /// <summary>
        /// Gets or sets the packet comment.
        /// </summary>
        public string? Comment { get; set; } = null;

        /// <summary>
        /// Gets or sets if the sender of the packet supports messaging.
        /// </summary>
        public bool? HasMessaging { get; set; } = false;

        /// <summary>
        /// Gets or sets the packet's destination address.
        /// </summary>
        public string? DestinationAddress { get; set; } = null;

        /// <summary>
        /// Gets or sets the software decode status for this packet.
        /// </summary>
        public PacketType DecodedType { get; set; } = PacketType.NotDecoded;

        /// <summary>
        /// Given an information field, this returns the <see cref="PacketType"/> of the APRS packet.
        /// </summary>
        /// <param name="informationField">A string encoded APRS Information Field.</param>
        /// <returns><see cref="PacketType"/> of the data type.</returns>
        public static PacketType GetDataType(string informationField)
        {
            if (informationField == null)
            {
                throw new ArgumentNullException(nameof(informationField));
            }

            // TODO Issue #67: This isn't always true.
            // '!' can come up to the 40th position.
            char dataTypeIdentifier = char.ToUpperInvariant(informationField[0]);

            return DataTypeMap[dataTypeIdentifier];
        }

        /// <summary>
        /// Returns the char for a given <see cref="PacketType"/>.
        /// </summary>
        /// <param name="type"><see cref="PacketType"/> to represent.</param>
        /// <returns>A char representing type.</returns>
        public static char GetTypeChar(PacketType type)
        {
            if (type == PacketType.DoNotUse || type == PacketType.Unused || type == PacketType.Unknown)
            {
                throw new ArgumentException("Used invalid PacketType " + type);
            }

            return DataTypeMap.Single(pair => pair.Value == type).Key;
        }

        /// <summary>
        /// Decodes a position from raw GPS location (NMEA formats)
        /// This expects a three letter type to start the string
        /// Supported formats:
        ///     None yet.
        /// </summary>
        /// <param name="rawGpsPacket">The full packet. Decoded.</param>
        public static void HandleRawGps(string rawGpsPacket)
        {
            if (rawGpsPacket == null)
            {
                throw new ArgumentNullException(nameof(rawGpsPacket));
            }
            else if (rawGpsPacket.Length < 6)
            {
                // 6 is the length of the identifier $GPxxx, so the string is invalid if it isn't at least that long
                throw new ArgumentException("rawGpsPacket should be 6 or more characters in length. Given length: " + rawGpsPacket.Length);
            }

            // Ensure start of identifier is valid
            if (!rawGpsPacket.Substring(0, 3).Equals("$GP", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentNullException("rawGpsPacket should have started with $GP. Given packet started with:" + rawGpsPacket.Substring(0, 3));
            }

            // Get type of packet
            NmeaType nmeaDataType = rawGpsPacket.Substring(3, 3).ToNmeaType();

            throw new NotImplementedException("handle RawGPSData");
        }

        /// <summary>
        /// Takes a string encoding of an APRS packet, decodes it, and saves it in to this object.
        /// </summary>
        /// <param name="encodedPacket">A string encoding of an AX.25 APRS packet.</param>
        public void Decode(string encodedPacket)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string version of this packet encoded in the requested type.
        /// </summary>
        /// <param name="encodeType">The <see cref="PacketType"/> of encoding to use.</param>
        /// <returns>AX.25 APRS packet as a string.</returns>
        public string Encode(PacketType encodeType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string of the APRS Information Field (or the AX.25 Information Field) given the proper encoding.
        /// </summary>
        /// <param name="encodeType">The <see cref="PacketType"/> of encoding to use.</param>
        /// <param name="timeType">The <see cref="TimestampType"/> of encoding for the timestamp to use. Optional, defaults to DHMz.</param>
        /// <returns>APRS Information Field as a string.</returns>
        public string EncodeInformationField(PacketType encodeType, TimestampType timeType = TimestampType.DHMz)
        {
            string encodedInfoField = string.Empty;

            switch (encodeType)
            {
                case PacketType.PositionWithTimestampWithMessaging:
                case PacketType.PositionWithTimestampNoMessaging:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += Timestamp!.Encode(timeType);
                    encodedInfoField += Position!.Encode();
                    encodedInfoField += Comment;
                    break;

                case PacketType.PositionWithoutTimestampNoMessaging:
                case PacketType.PositionWithoutTimestampWithMessaging:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += Position!.Encode();
                    encodedInfoField += Comment;
                    break;

                case PacketType.MaidenheadGridLocatorBeacon:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += Position!.EncodeGridsquare(6, false);
                    encodedInfoField += ']';
                    if (Comment != null && Comment.Length > 0)
                    {
                        encodedInfoField += ' ';
                        encodedInfoField += Comment;
                    }

                    break;

                case PacketType.Status:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += Position!.EncodeGridsquare(6, true);
                    if (Comment != null && Comment.Length > 0)
                    {
                        encodedInfoField += ' ';
                        encodedInfoField += Comment;
                    }

                    break;

                default: throw new NotImplementedException();
            }

            return encodedInfoField;
        }

        /// <summary>
        /// Takes a string representing the AX.25 Information Field or (as it's called in this case) the APRS Information Field
        /// and populates this object with the information therein.
        /// </summary>
        /// <param name="informationField">string representation of the APRS Information Field.</param>
        public void DecodeInformationField(string informationField)
        {
            if (informationField == null)
            {
                throw new ArgumentNullException(nameof(informationField));
            }
            else if (informationField.Length == 0)
            {
                throw new ArgumentException("Empty string argument");
            }

            DecodedType = GetDataType(informationField);

            switch (DecodedType)
            {
                case PacketType.CurrentMicEData:
                    throw new NotImplementedException("handle currnet Mic-E Data (Rev 0 beta)");
                case PacketType.OldMicEData:
                    throw new NotImplementedException("handle old Mic-E Data (Rev 0 beta)");

                // Handle position without timestamp (no APRS messaging), or Ultimeter 2000 WX Station
                case PacketType.PositionWithoutTimestampNoMessaging:
                    {
                        HasMessaging = false;
                        Match match = Regex.Match(informationField, RegexStrings.PositionWithoutTimestamp);
                        match.AssertSuccess(PacketType.PositionWithoutTimestampNoMessaging, nameof(informationField));

                        Position = new Position(match.Groups[1].Value);

                        if (match.Groups[6].Success)
                        {
                            Comment = match.Groups[6].Value;
                        }

                        break;
                    }

                case PacketType.PeetBrosUIIWeatherStation:
                    throw new NotImplementedException("handle Peet Bros U-II Weather Station");
                case PacketType.RawGPSData:
                    HandleRawGps(informationField);
                    break;

                case PacketType.AgreloDFJrMicroFinder:
                    throw new NotImplementedException("handle Agrelo DFJr / MicroFinder");
                case PacketType.MapFeature:
                    throw new NotImplementedException("handle Reserved - Map Feature");
                case PacketType.OldMicEDataCurrentTMD700:
                    throw new NotImplementedException("handle Old Mic-E Data (but Current data for TM-D700");
                case PacketType.Item:
                    throw new NotImplementedException("handle Item");
                case PacketType.ShelterDataWithTime:
                    throw new NotImplementedException("handle Reserved - Shelter data with time");
                case PacketType.InvalidOrTestData:
                    throw new NotImplementedException("handle Invalid data or test data");
                case PacketType.SpaceWeather:
                    throw new NotImplementedException("handle Reserved - Space weather");
                case PacketType.PositionWithTimestampNoMessaging:
                    HandlePositionWithTimestamp(informationField, false);
                    break;

                case PacketType.Message:
                    throw new NotImplementedException("handle Message");
                case PacketType.Object:
                    throw new NotImplementedException("handle Object");
                case PacketType.StationCapabilities:
                    throw new NotImplementedException("handle Station capabilities");
                case PacketType.PositionWithoutTimestampWithMessaging:
                    // handle Position without timestamp (with APRS messaging)
                    HasMessaging = true;
                    throw new NotImplementedException("handle Position without timestamp (with APRS messaging)");
                case PacketType.Status:
                    {
                        Position = new Position();
                        Match match = Regex.Match(informationField, RegexStrings.StatusWithMaidenheadAndComment);
                        match.AssertSuccess(PacketType.Status, nameof(informationField));

                        Position.DecodeMaidenhead(match.Groups[1].Value);

                        if (match.Groups[4].Success)
                        {
                            Comment = match.Groups[4].Value;
                        }
                    }

                    break;

                case PacketType.Query:
                    throw new NotImplementedException("handle Query");
                case PacketType.PositionWithTimestampWithMessaging:
                    HandlePositionWithTimestamp(informationField, true);
                    break;

                case PacketType.TelemetryData:
                    throw new NotImplementedException("handle Telemetry data");

                case PacketType.MaidenheadGridLocatorBeacon:
                    {
                        Match match = Regex.Match(informationField, RegexStrings.MaidenheadGridLocatorBeacon);
                        match.AssertSuccess(PacketType.MaidenheadGridLocatorBeacon, nameof(informationField));

                        Position = new Position();
                        Position.DecodeMaidenhead(match.Groups[1].Value);

                        if (!string.IsNullOrEmpty(match.Groups[3].Value))
                        {
                            Comment = match.Groups[3].Value;
                        }
                    }

                    break;

                case PacketType.WeatherReport: // TODO raw weather reports vs positionless?
                    // handle Weather report(without position)
                    Timestamp = new Timestamp(informationField.Substring(1, 8));
                    throw new NotImplementedException("handle Weather report (without position)");
                case PacketType.CurrentMicEDataNotTMD700:
                    throw new NotImplementedException("handle Current Mic-E Data (not used in TM-D700");
                case PacketType.UserDefinedAPRSPacketFormat:
                    throw new NotImplementedException("handle User-Defined APRS packet format");
                case PacketType.ThirdPartyTraffic:
                    throw new NotImplementedException("handle Third-party traffic");
                case PacketType.DoNotUse:
                    throw new NotImplementedException("Do not use");
                case PacketType.Unused:
                    throw new NotImplementedException("Unused");
                case PacketType.Unknown:
                    throw new NotImplementedException("Unknown");
                default:
                    throw new ArgumentOutOfRangeException(nameof(informationField));
            }
        }

        /// <summary>
        /// Logic for positin with timestamp with and without messaging
        /// condensed here to save copy/paste bugs.
        /// </summary>
        /// <param name="informationField">The packet info field to decode.</param>
        /// <param name="hasMessaging">true if this packet represents messaging capabilities.</param>
        private void HandlePositionWithTimestamp(string informationField, bool hasMessaging)
        {
            Match match = Regex.Match(informationField, RegexStrings.PositionWithTimestamp);
            match.AssertSuccess(
                hasMessaging ?
                    PacketType.PositionWithTimestampWithMessaging :
                    PacketType.PositionWithTimestampNoMessaging,
                nameof(informationField));

            HasMessaging = hasMessaging;

            Timestamp = new Timestamp(match.Groups[2].Value);

            Position = new Position(match.Groups[3].Value);

            if (match.Groups[8].Success)
            {
                Comment = match.Groups[8].Value;
            }
        }
    }
}
