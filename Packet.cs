using System;

namespace APaRSer
{
    /// <summary>
    /// A representation of an APRS Packet.
    /// Does decoding of an APRS packet as a string.
    /// </summary>
    public class Packet
    {
        public Timestamp timestamp;
        public bool HasMessaging = false;
        public string DestinationAddress;
        Type DecodedType = Type.NotDecoded;

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
            PositionWithoutTimeStampWithMessaging,
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
        /// <param name="encodedPacket">A string encoding of an APRS packet</param>
        public void Decode(string encodedPacket)
        {
            throw new NotImplementedException();
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
                    throw new NotImplementedException("handle position without timestamp (no APRS messaging), or Ultimeter 2000 WX Station");
                case Type.PeetBrosUIIWeatherStation:
                    throw new NotImplementedException("handle Peet Bros U-II Weather Station");
                case Type.RawGPSData:
                    throw new NotImplementedException("handle Raw GPS data or Ultimeter 2000");
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
                    // handle Position with timestamp (no APRS messaging)
                    HasMessaging = false;
                    timestamp = new Timestamp(informationField.Substring(1, 7));
                    throw new NotImplementedException("handle Position with timestamp (no APRS messaging)");
                case Type.Message:
                    throw new NotImplementedException("handle Message");
                case Type.Object:
                    throw new NotImplementedException("handle Object");
                case Type.StationCapabilities:
                    throw new NotImplementedException("handle Station capabilities");
                case Type.PositionWithoutTimeStampWithMessaging:
                    // handle Position without timestamp (with APRS messaging)
                    HasMessaging = true;
                    throw new NotImplementedException("handle Position without timestamp (with APRS messaging)");
                case Type.Status:
                    throw new NotImplementedException("handle Status");
                case Type.Query:
                    throw new NotImplementedException("handle Query");
                case Type.PositionWithTimestampWithMessaging:
                    // handle Position with timestamp (with APRS messaging)
                    HasMessaging = true;
                    timestamp = new Timestamp(informationField.Substring(1, 7));
                    throw new NotImplementedException("handle Position with timestamp (with APRS messaging)");
                case Type.TelemetryData:
                    throw new NotImplementedException("handle Telemetry data");
                case Type.MaidenheadGridLocatorBeacon:
                    throw new NotImplementedException("handle Maidenhead grid locator beacon (obsolete)");
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

        private Type GetDataType(string informationField)
        {
            // TODO: This isn't always true.
            // '!' can come up to the 40th position.
            char dataTypeIdentifier = informationField[0];

            switch (dataTypeIdentifier)
            {
                case (char)0x1c: return Type.CurrentMicEData;
                case (char)0x1d: return Type.OldMicEData;
                case '!': return Type.PositionWithoutTimestampNoMessaging;
                case '\"': return Type.Unused;
                case '#': return Type.PeetBrosUIIWeatherStation;
                case '$': return Type.RawGPSData;
                case '%': return Type.AgreloDFJrMicroFinder;
                case '&': return Type.MapFeature;
                case '\'': return Type.OldMicEDataCurrentTMD700;
                case '(': return Type.Unused;
                case ')': return Type.Item;
                case '*': return Type.PeetBrosUIIWeatherStation;
                case '+': return Type.ShelterDataWithTime;
                case ',': return Type.InvalidOrTestData;
                case '-': return Type.Unused;
                case '.': return Type.SpaceWeather;
                case '/': return Type.PositionWithTimestampNoMessaging;
                case ':': return Type.Message;
                case ';': return Type.Object;
                case '<': return Type.StationCapabilities;
                case '=': return Type.PositionWithoutTimestampNoMessaging;
                case '>': return Type.Status;
                case '?': return Type.Query;
                case '@': return Type.PositionWithTimestampWithMessaging;
                case 'T': return Type.TelemetryData;
                case '[': return Type.MaidenheadGridLocatorBeacon;
                case '\\': return Type.Unused;
                case ']': return Type.Unused;
                case '^': return Type.Unused;
                case '_': return Type.WeatherReport;
                case '`': return Type.CurrentMicEDataNotTMD700;
                case '{': return Type.UserDefinedAPRSPacketFormat;
                case '}': return Type.ThirdPartyTraffic;
                default:
                    char upperDataIdentifier = Char.ToUpperInvariant(dataTypeIdentifier);
                    if (((upperDataIdentifier >= 'A') &&
                        (upperDataIdentifier <= 'S')) ||
                        ((upperDataIdentifier >= 'U') &&
                        (upperDataIdentifier <= 'Z')) ||
                        ((upperDataIdentifier >= '0') &&
                        (upperDataIdentifier <= '9')))
                    {
                        return Type.DoNotUse;
                    }
                    else
                    {
                        return Type.Unknown;
                    }
            }
        }
    }
}
