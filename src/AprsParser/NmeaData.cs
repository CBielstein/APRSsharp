namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the NMEA data in an APRS packets.
    /// </summary>
    public sealed class NmeaData
    {
        /// <summary>
        /// Maps three-character strings to RawGpsType values.
        /// </summary>
        private static readonly Dictionary<string, NmeaType> RawGpsTypeMap = new Dictionary<string, NmeaType>()
        {
            { "GGA", NmeaType.GGA },
            { "GLL", NmeaType.GLL },
            { "RMC", NmeaType.RMC },
            { "VTG", NmeaType.VTG },
            { "WPT", NmeaType.WPT },
            { "WPL", NmeaType.WPT },
        };

        /// <summary>
        /// Determines the <see cref="NmeaType"/> of a raw GPS packet determined by a string.
        /// If the string is length 3, the three letters are taken as is.
        /// If the string is length 6 or longer, the indentifier is expected in the place dictated by the NMEA formats.
        /// </summary>
        /// <param name="nmeaInput">String of length 3 identifying a raw GPS type or an entire NMEA string.</param>
        /// <returns>The raw GPS <see cref="NmeaType"/> represented by the argument.</returns>
        public static NmeaType GetType(string nmeaInput)
        {
            if (nmeaInput == null)
            {
                throw new ArgumentNullException(nameof(nmeaInput));
            }

            string nmeaIdentifier;

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
                return NmeaType.Unknown;
            }
        }
    }
}
