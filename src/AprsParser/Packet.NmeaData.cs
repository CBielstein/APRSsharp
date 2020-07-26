using System;
using System.Collections.Generic;

namespace AprsSharp.Parsers.Aprs
{
    public partial class Packet
    {
        public class NmeaData
        {
            /// <summary>
            /// Used to specify format during decode of raw GPS data packets
            /// </summary>
            public enum Type
            {
                /// <summary>
                /// Not yet decoded
                /// </summary>
                NotDecoded,
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
                /// WPT: Way Point Location (also WPL)
                /// </summary>
                WPT,
                /// <summary>
                /// Not supported/known type
                /// </summary>
                Unknown,
            }

            /// <summary>
            /// Determines the type of a raw GPS packet determined by a string.
            /// If the string is length 3, the three letters are taken as is.
            /// If the string is length 6 or longer, the indentifier is expected in the place dictated by the NMEA formats
            /// </summary>
            /// <param name="nmeaInput">String of length 3 identifying a raw GPS type or an entire NMEA string</param>
            /// <returns>The raw GPS type represented by the argument</returns>
            public static Type GetType(string nmeaInput)
            {
                if (nmeaInput == null)
                {
                    throw new ArgumentNullException();
                }

                string nmeaIdentifier = null;

                if (nmeaInput.Length == 3)
                {
                    nmeaIdentifier = nmeaInput.ToUpperInvariant();
                }
                else if (nmeaInput.Length >= 6)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new ArgumentException("rawGpsIdentifier should be exactly 3 characters or at least 6. Given: " + nmeaInput.Length);
                }

                try
                {
                    return RawGpsTypeMap[nmeaIdentifier];
                }
                catch (KeyNotFoundException)
                {
                    return Type.Unknown;
                }
            }

            // Maps three-character strings to RawGpsType values
            private static Dictionary<string, Type> RawGpsTypeMap = new Dictionary<string, Type>()
            {
                {"GGA", Type.GGA },
                {"GLL", Type.GLL },
                {"RMC", Type.RMC },
                {"VTG", Type.VTG },
                {"WPT", Type.WPT },
                {"WPL", Type.WPT },
            };
            
        }
    }
}
