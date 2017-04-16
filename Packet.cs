using System;
using System.Collections.Generic;
using System.Linq;

namespace APaRSer
{
    /// <summary>
    /// A representation of an APRS Packet.
    /// Does decoding of an APRS packet as a string.
    /// </summary>
    public class Packet
    {
        public Timestamp timestamp = null;
        public Position position = null;
        public string comment = null;
        public bool HasMessaging = false;
        public string DestinationAddress = null;
        Type DecodedType = Type.NotDecoded;
        RawGpsType DecodedRawGpsType = RawGpsType.NotDecoded;

        public enum Type
        {
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
            /// <summary>
            /// This packet was not decoded
            /// </summary>
            NotDecoded,
        }

        /// <summary>
        /// Used to specify format during decode of raw GPS data packets
        /// </summary>
        public enum RawGpsType
        {
            /// <summary>
            /// GGA: Global Position System Fix Data
            /// </summary>
            GGA,
            /// <summary>
            /// GLL: Geographic Position, Latitude/Longitude Data
            /// </summary>
            GLL,
            /// <summary>
            /// RMC: Recommended Minimum Specific GPS/Transit Data
            /// </summary>
            RMC,
            /// <summary>
            /// VTG: Velocity and Track Data
            /// </summary>
            VTG,
            /// <summary>
            /// WPT: Way Point Location
            /// </summary>
            WPT,
            /// <summary>
            /// Not yet decoded
            /// </summary>
            NotDecoded,
            /// <summary>
            /// Not supported/known type
            /// </summary>
            Unknown,
        }

        /// <summary>
        /// Constructs a new Packet object by decoding the encodedPacket string.
        /// Really just a shortcut for calling the Decode function.
        /// </summary>
        /// <param name="encodedPacket">A string encoding of an APRS packet</param>
        public Packet(string encodedPacket)
        {
            Decode(encodedPacket);
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Packet() { }

        /// <summary>
        /// Takes a string encoding of an APRS packet, decodes it, and saves it in to this object.
        /// </summary>
        /// <param name="encodedPacket">A string encoding of an AX.25 APRS packet</param>
        public void Decode(string encodedPacket)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string version of this packet encoded in the requested type.
        /// </summary>
        /// <param name="encodeType">The type of encoding to use</param>
        /// <returns>AX.25 APRS packet as a string</returns>
        public string Encode(Type encodeType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string of the APRS Information Field (or the AX.25 Information Field) given the proper encoding
        /// </summary>
        /// <param name="encodeType">The type of encoding to use</param>
        /// <returns>APRS Information Field as a string</returns>
        private string EncodeInformationField(Type encodeType)
        {
            return EncodeInformationField(encodeType, Timestamp.Type.DHMz);
        }

        /// <summary>
        /// Returns a string of the APRS Information Field (or the AX.25 Information Field) given the proper encoding
        /// </summary>
        /// <param name="encodeType">The type of encoding to use</param>
        /// <param name="timeType">The type of encoding for the timestamp to use</param>
        /// <returns>APRS Information Field as a string</returns>
        private string EncodeInformationField(Type encodeType, Timestamp.Type timeType)
        {
            string encodedInfoField = string.Empty;


            switch (encodeType)
            {
                case Type.PositionWithTimestampWithMessaging:
                case Type.PositionWithTimestampNoMessaging:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += timestamp.Encode(timeType);
                    encodedInfoField += position.Encode();
                    encodedInfoField += comment;
                    break;

                case Type.PositionWithoutTimestampNoMessaging:
                case Type.PositionWithoutTimestampWithMessaging:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += position.Encode();
                    encodedInfoField += comment;
                    break;

                case Type.MaidenheadGridLocatorBeacon:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += position.EncodeGridsquare(6, false);
                    encodedInfoField += ']';
                    if (comment != null && comment.Length > 0)
                    {
                        encodedInfoField += ' ';
                        encodedInfoField += comment;
                    }
                    break;

                case Type.Status:
                    encodedInfoField += GetTypeChar(encodeType);
                    encodedInfoField += position.EncodeGridsquare(6, true);
                    if (comment != null && comment.Length > 0)
                    {
                        encodedInfoField += ' ';
                        encodedInfoField += comment;
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
        /// <param name="informationField">string representation of the APRS Information Field</param>
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

            DecodedType = GetDataType(informationField);

            switch (DecodedType)
            {
                case Type.CurrentMicEData:
                    throw new NotImplementedException("handle currnet Mic-E Data (Rev 0 beta)");
                case Type.OldMicEData:
                    throw new NotImplementedException("handle old Mic-E Data (Rev 0 beta)");
                case Type.PositionWithoutTimestampNoMessaging:
                    // handle position without timestamp (no APRS messaging), or Ultimeter 2000 WX Station
                    HasMessaging = false;
                    position = new Position(informationField.Substring(1, 19));

                    if (informationField.Length > 20)
                    {
                        comment = informationField.Substring(20);
                    }

                    break;

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
                        position = new Position();
                        int endGridsquare = informationField.IndexOf(' ');
                        if (endGridsquare == -1)
                        {
                            position.DecodeMaidenhead(informationField.Substring(1));
                        }
                        else
                        {
                            position.DecodeMaidenhead(informationField.Substring(1, endGridsquare - 1));
                        }

                        if (endGridsquare + 1 < informationField.Length)
                        {
                            comment = informationField.Substring(endGridsquare + 1);
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
                    position = new Position();

                    {
                        position = new Position();
                        int endGridsquare = informationField.IndexOf(']');
                        position.DecodeMaidenhead(informationField.Substring(1, endGridsquare - 1));

                        if (endGridsquare + 1 < informationField.Length)
                        {
                            comment = informationField.Substring(endGridsquare + 1);
                        }
                    }

                    break;

                case Type.WeatherReport: // TODO raw weather reports vs positionless?
                    // handle Weather report(without position)
                    timestamp = new Timestamp(informationField.Substring(1, 8));
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
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Maps a type identifying character to a packet Type
        private static Dictionary<char, Type> DataTypeMap = new Dictionary<char, Type>()
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

        // Maps three-character strings to RawGpsType values
        private static Dictionary<string, RawGpsType> RawGpsTypeMap = new Dictionary<string, RawGpsType>()
        {
            {"GGA", RawGpsType.GGA },
            {"GLL", RawGpsType.GLL },
            {"RMC", RawGpsType.RMC },
            {"VTG", RawGpsType.VTG },
            {"WPT", RawGpsType.WPT },
        };

        /// <summary>
        /// Determines the type of a raw GPS packet determined by a 3 char string
        /// </summary>
        /// <param name="rawGpsIdentifier">String of length 3 identifying a raw GPS type</param>
        /// <returns>The raw GPS type represented by rawGpsIdentifier</returns>
        private RawGpsType GetRawGpsType(string rawGpsIdentifier)
        {
            if (rawGpsIdentifier == null)
            {
                throw new ArgumentNullException();
            }
            else if (rawGpsIdentifier.Length != 3)
            {
                throw new ArgumentException("rawGpsIdentifier should be 3 characters. Given: " + rawGpsIdentifier.Length);
            }

            return RawGpsTypeMap[rawGpsIdentifier];
        }

        /// <summary>
        /// Given an information field, this returns the Type of the APRS packet
        /// </summary>
        /// <param name="informationField">A string encoded APRS Information Field</param>
        /// <returns>Packet.Type of the data type</returns>
        private Type GetDataType(string informationField)
        {
            // TODO: This isn't always true.
            // '!' can come up to the 40th position.
            char dataTypeIdentifier = char.ToUpperInvariant(informationField[0]);

            return DataTypeMap[dataTypeIdentifier];
        }

        /// <summary>
        /// Returns the char for a given Packet.Type
        /// </summary>
        /// <param name="type">Type to represent</param>
        /// <returns>A char representing type</returns>
        private char GetTypeChar(Type type)
        {
            if (type == Type.DoNotUse || type == Type.Unused || type == Type.Unknown)
            {
                throw new ArgumentException("Used invalid Type " + type);
            }

            IEnumerable<char> keys = DataTypeMap.Keys.Where(x => DataTypeMap[x] == type);
            return keys.Single();
        }

        /// <summary>
        /// Logic for positin with timestamp with and without messaging
        /// condensed here to save copy/paste bugs
        /// </summary>
        /// <param name="informationField">The packet info field to decode</param>
        /// <param name="hasMessaging">true if this packet represents messaging capabilities</param>
        private void HandlePositionWithTimestamp(string informationField, bool hasMessaging)
        {
            HasMessaging = hasMessaging;

            timestamp = new Timestamp(informationField.Substring(1, 7));

            position = new Position(informationField.Substring(8, 19));

            if (informationField.Length > 27)
            {
                comment = informationField.Substring(27);
            }
        }

        /// <summary>
        /// Decodes a position from raw GPS location (NMEA formats)
        /// This expects a three letter type to start the string
        /// Supported formats:
        ///     None yet
        /// </summary>
        /// <param name="rawGpsPacket">The full packet. Decoded</param>
        /// <param name="rawGpsType"></param>
        /// <param name="rawGpsTimestamp"></param>
        public void HandleRawGps(string rawGpsPacket)
        {
            DecodedRawGpsType = Packet.RawGpsType.Unknown;

            if (rawGpsPacket == null)
            {
                throw new ArgumentNullException();
            }
            // 6 is the length of the identifier $GPxxx, so the string is invalid if it isn't at least that long
            else if (rawGpsPacket.Length < 6)
            {
                throw new ArgumentException("rawGpsPacket should be 6 or more characters in length. Given length: " + rawGpsPacket.Length);
            }

            string upperRawPacket = rawGpsPacket.ToUpperInvariant();

            // Ensure start of identifier is valid
            if (!upperRawPacket.Substring(0, 3).Equals("$GP"))
            {
                throw new ArgumentNullException("rawGpsPacket should have started with $GP. Given packet started with:" + upperRawPacket.Substring(0, 3));
            }

            // Get type of packet
            DecodedRawGpsType = GetRawGpsType(upperRawPacket.Substring(3, 3));
        }
    }
}
