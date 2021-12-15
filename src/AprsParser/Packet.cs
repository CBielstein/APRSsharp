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
        /// Maps a type-identifying character to a packet Type.
        /// </summary>
        private static readonly Dictionary<char, Type> DataTypeMap = new Dictionary<char, Type>()
        {
            { (char)0x1c, Type.CurrentMicEData },
            { (char)0x1d, Type.OldMicEData },
            { '!', Type.PositionWithoutTimestampNoMessaging },
            { '\"', Type.Unused },
            { '#', Type.PeetBrosUIIWeatherStation },
            { '$', Type.RawGPSData },
            { '%', Type.AgreloDFJrMicroFinder },
            { '&', Type.MapFeature },
            { '\'', Type.OldMicEDataCurrentTMD700 },
            { '(', Type.Unused },
            { ')', Type.Item },
            { '*', Type.PeetBrosUIIWeatherStation },
            { '+', Type.ShelterDataWithTime },
            { ',', Type.InvalidOrTestData },
            { '-', Type.Unused },
            { '.', Type.SpaceWeather },
            { '/', Type.PositionWithTimestampNoMessaging },
            { ':', Type.Message },
            { ';', Type.Object },
            { '<', Type.StationCapabilities },
            { '=', Type.PositionWithoutTimestampWithMessaging },
            { '>', Type.Status },
            { '?', Type.Query },
            { '@', Type.PositionWithTimestampWithMessaging },
            { 'T', Type.TelemetryData },
            { '[', Type.MaidenheadGridLocatorBeacon },
            { '\\', Type.Unused },
            { ']', Type.Unused },
            { '^', Type.Unused },
            { '_', Type.WeatherReport },
            { '`', Type.CurrentMicEDataNotTMD700 },
            { '{', Type.UserDefinedAPRSPacketFormat },
            { '}', Type.ThirdPartyTraffic },
            { 'A', Type.DoNotUse },
            { 'S', Type.DoNotUse },
            { 'U', Type.DoNotUse },
            { 'Z', Type.DoNotUse },
            { '0', Type.DoNotUse },
            { '9', Type.DoNotUse },
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
        /// The APRS packet type.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// This packet was not decoded
            /// </summary>
            NotDecoded,

            /// <summary>
            /// Current Mic-E Data (Rev 0 beta)
            /// </summary>
            CurrentMicEData,

            /// <summary>
            /// Old Mic-E Data (Rev 0 beta)
            /// </summary>
            OldMicEData,

            /// <summary>
            /// Position without timestamp (no APRS messaging), or Ultimeter 2000 WX Station
            /// </summary>
            PositionWithoutTimestampNoMessaging,

            /// <summary>
            /// Peet Bros U-II Weather Station
            /// </summary>
            PeetBrosUIIWeatherStation,

            /// <summary>
            /// Raw GPS data or Ultimeter 2000
            /// </summary>
            RawGPSData,

            /// <summary>
            /// Agrelo DFJr / MicroFinder
            /// </summary>
            AgreloDFJrMicroFinder,

            /// <summary>
            /// [Reserved - Map Feature]
            /// </summary>
            MapFeature,

            /// <summary>
            /// Old Mic-E Data (but Current data for TM-D700)
            /// </summary>
            OldMicEDataCurrentTMD700,

            /// <summary>
            /// Item
            /// </summary>
            Item,

            /// <summary>
            /// [Reserved - shelter data with time]
            /// </summary>
            ShelterDataWithTime,

            /// <summary>
            /// Invalid data or test data
            /// </summary>
            InvalidOrTestData,

            /// <summary>
            /// [Reserved - Space Weather]
            /// </summary>
            SpaceWeather,

            /// <summary>
            /// Unused
            /// </summary>
            Unused,

            /// <summary>
            /// Position with timestamp (no APRS messaging)
            /// </summary>
            PositionWithTimestampNoMessaging,

            /// <summary>
            /// Message
            /// </summary>
            Message,

            /// <summary>
            /// Object
            /// </summary>
            [field:System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720", Justification = "APRS spec uses the word 'object', so it is appropriate here")]
            Object,

            /// <summary>
            /// Station Capabilities
            /// </summary>
            StationCapabilities,

            /// <summary>
            /// Position without timestamp (with APRS messaging)
            /// </summary>
            PositionWithoutTimestampWithMessaging,

            /// <summary>
            /// Status
            /// </summary>
            Status,

            /// <summary>
            /// Query
            /// </summary>
            Query,

            /// <summary>
            /// [Do not use]
            /// </summary>
            DoNotUse,

            /// <summary>
            /// Positionwith timestamp (with APRS messaging)
            /// </summary>
            PositionWithTimestampWithMessaging,

            /// <summary>
            /// Telemetry data
            /// </summary>
            TelemetryData,

            /// <summary>
            /// Maidenhead grid locator beacon (obsolete)
            /// </summary>
            MaidenheadGridLocatorBeacon,

            /// <summary>
            /// Weather Report (without position)
            /// </summary>
            WeatherReport,

            /// <summary>
            /// Current Mic-E Data (not used in TM-D700)
            /// </summary>
            CurrentMicEDataNotTMD700,

            /// <summary>
            /// User-Defined APRS packet format
            /// </summary>
            UserDefinedAPRSPacketFormat,

            /// <summary>
            /// [Do not use - TNC stream switch character]
            /// </summary>
            DoNotUseTNSStreamSwitchCharacter,

            /// <summary>
            /// Third-party traffic
            /// </summary>
            ThirdPartyTraffic,

            /// <summary>
            /// Not a recognized symbol
            /// </summary>
            Unknown,
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
        public Type DecodedType { get; set; } = Type.NotDecoded;

        /// <summary>
        /// Gets or sets the packet's raw NMEA data.
        /// </summary>
        public NmeaData? RawNmeaData { get; set; } = null;

        /// <summary>
        /// Given an information field, this returns the Type of the APRS packet.
        /// </summary>
        /// <param name="informationField">A string encoded APRS Information Field.</param>
        /// <returns>Packet.Type of the data type.</returns>
        public static Type GetDataType(string informationField)
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
        /// Returns the char for a given Packet.Type.
        /// </summary>
        /// <param name="type">Type to represent.</param>
        /// <returns>A char representing type.</returns>
        public static char GetTypeChar(Type type)
        {
            if (type == Type.DoNotUse || type == Type.Unused || type == Type.Unknown)
            {
                throw new ArgumentException("Used invalid Type " + type);
            }

            IEnumerable<char> keys = DataTypeMap.Keys.Where(x => DataTypeMap[x] == type);
            return keys.Single();
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
            NmeaData.Type nmeaDataType = NmeaData.GetType(rawGpsPacket.Substring(3, 3));

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
        /// <param name="encodeType">The type of encoding to use.</param>
        /// <returns>AX.25 APRS packet as a string.</returns>
        public string Encode(Type encodeType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string of the APRS Information Field (or the AX.25 Information Field) given the proper encoding.
        /// </summary>
        /// <param name="encodeType">The type of encoding to use.</param>
        /// <param name="timeType">The type of encoding for the timestamp to use. Optional, defaults to DHMz.</param>
        /// <returns>APRS Information Field as a string.</returns>
        public string EncodeInformationField(Type encodeType, Timestamp.Type timeType = Timestamp.Type.DHMz)
        {
            string encodedInfoField = string.Empty;

            switch (encodeType)
            {
                case Type.PositionWithTimestampWithMessaging:
                case Type.PositionWithTimestampNoMessaging:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += Timestamp!.Encode(timeType);
                    encodedInfoField += Position!.Encode();
                    encodedInfoField += Comment;
                    break;

                case Type.PositionWithoutTimestampNoMessaging:
                case Type.PositionWithoutTimestampWithMessaging:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += Position!.Encode();
                    encodedInfoField += Comment;
                    break;

                case Type.MaidenheadGridLocatorBeacon:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += Position!.EncodeGridsquare(6, false);
                    encodedInfoField += ']';
                    if (Comment != null && Comment.Length > 0)
                    {
                        encodedInfoField += ' ';
                        encodedInfoField += Comment;
                    }

                    break;

                case Type.Status:
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
                case Type.CurrentMicEData:
                    throw new NotImplementedException("handle currnet Mic-E Data (Rev 0 beta)");
                case Type.OldMicEData:
                    throw new NotImplementedException("handle old Mic-E Data (Rev 0 beta)");

                // Handle position without timestamp (no APRS messaging), or Ultimeter 2000 WX Station
                case Type.PositionWithoutTimestampNoMessaging:
                    {
                        HasMessaging = false;
                        Match match = Regex.Match(informationField, RegexStrings.PositionWithoutTimestamp);
                        match.AssertSuccess(Type.PositionWithoutTimestampNoMessaging, nameof(informationField));

                        Position = new Position(match.Groups[1].Value);

                        if (match.Groups[6].Success)
                        {
                            Comment = match.Groups[6].Value;
                        }

                        break;
                    }

                case Type.PeetBrosUIIWeatherStation:
                    throw new NotImplementedException("handle Peet Bros U-II Weather Station");
                case Type.RawGPSData:
                    HandleRawGps(informationField);
                    break;

                case Type.AgreloDFJrMicroFinder:
                    throw new NotImplementedException("handle Agrelo DFJr / MicroFinder");
                case Type.MapFeature:
                    throw new NotImplementedException("handle Reserved - Map Feature");
                case Type.OldMicEDataCurrentTMD700:
                    throw new NotImplementedException("handle Old Mic-E Data (but Current data for TM-D700");
                case Type.Item:
                    throw new NotImplementedException("handle Item");
                case Type.ShelterDataWithTime:
                    throw new NotImplementedException("handle Reserved - Shelter data with time");
                case Type.InvalidOrTestData:
                    throw new NotImplementedException("handle Invalid data or test data");
                case Type.SpaceWeather:
                    throw new NotImplementedException("handle Reserved - Space weather");
                case Type.PositionWithTimestampNoMessaging:
                    HandlePositionWithTimestamp(informationField, false);
                    break;

                case Type.Message:
                    throw new NotImplementedException("handle Message");
                case Type.Object:
                    throw new NotImplementedException("handle Object");
                case Type.StationCapabilities:
                    throw new NotImplementedException("handle Station capabilities");
                case Type.PositionWithoutTimestampWithMessaging:
                    // handle Position without timestamp (with APRS messaging)
                    HasMessaging = true;
                    throw new NotImplementedException("handle Position without timestamp (with APRS messaging)");
                case Type.Status:
                    {
                        Position = new Position();
                        Match match = Regex.Match(informationField, RegexStrings.StatusWithMaidenheadAndComment);
                        match.AssertSuccess(Type.Status, nameof(informationField));

                        Position.DecodeMaidenhead(match.Groups[1].Value);

                        if (match.Groups[4].Success)
                        {
                            Comment = match.Groups[4].Value;
                        }
                    }

                    break;

                case Type.Query:
                    throw new NotImplementedException("handle Query");
                case Type.PositionWithTimestampWithMessaging:
                    HandlePositionWithTimestamp(informationField, true);
                    break;

                case Type.TelemetryData:
                    throw new NotImplementedException("handle Telemetry data");

                case Type.MaidenheadGridLocatorBeacon:
                    {
                        Match match = Regex.Match(informationField, RegexStrings.MaidenheadGridLocatorBeacon);
                        match.AssertSuccess(Type.MaidenheadGridLocatorBeacon, nameof(informationField));

                        Position = new Position();
                        Position.DecodeMaidenhead(match.Groups[1].Value);

                        if (!string.IsNullOrEmpty(match.Groups[3].Value))
                        {
                            Comment = match.Groups[3].Value;
                        }
                    }

                    break;

                case Type.WeatherReport: // TODO raw weather reports vs positionless?
                    // handle Weather report(without position)
                    Timestamp = new Timestamp(informationField.Substring(1, 8));
                    throw new NotImplementedException("handle Weather report (without position)");
                case Type.CurrentMicEDataNotTMD700:
                    throw new NotImplementedException("handle Current Mic-E Data (not used in TM-D700");
                case Type.UserDefinedAPRSPacketFormat:
                    throw new NotImplementedException("handle User-Defined APRS packet format");
                case Type.ThirdPartyTraffic:
                    throw new NotImplementedException("handle Third-party traffic");
                case Type.DoNotUse:
                    throw new NotImplementedException("Do not use");
                case Type.Unused:
                    throw new NotImplementedException("Unused");
                case Type.Unknown:
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
                    Type.PositionWithTimestampWithMessaging :
                    Type.PositionWithTimestampNoMessaging,
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
