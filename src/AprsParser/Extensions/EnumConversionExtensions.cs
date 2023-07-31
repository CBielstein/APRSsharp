namespace AprsSharp.AprsParser.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for conversion of enum types.
    /// </summary>
    public static class EnumConversionExtensions
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
        /// Converts a char to its corresponding <see cref="PacketType"/>.
        /// </summary>
        /// <param name="typeIdentifier">A char representation of <see cref="PacketType"/>.</param>
        /// <returns><see cref="PacketType"/> of the info field, <see cref="PacketType.Unknown"/> if not a valid mapping.</returns>
        public static PacketType ToPacketType(this char typeIdentifier) => DataTypeMap.GetValueOrDefault(char.ToUpperInvariant(typeIdentifier), PacketType.Unknown);

        /// <summary>
        /// Converts to the char representation of a given <see cref="PacketType"/>.
        /// </summary>
        /// <param name="type"><see cref="PacketType"/> to represent.</param>
        /// <returns>A char representing type.</returns>
        public static char ToChar(this PacketType type) => DataTypeMap.First(pair => pair.Value == type).Key;

        /// <summary>
        /// Determines the <see cref="NmeaType"/> of a raw GPS packet determined by a string.
        /// If the string is length 3, the three letters are taken as is.
        /// If the string is length 6 or longer, the indentifier is expected in the place dictated by the NMEA formats.
        /// </summary>
        /// <param name="nmeaInput">String of length 3 identifying a raw GPS type or an entire NMEA string.</param>
        /// <returns>The raw GPS <see cref="NmeaType"/> represented by the argument.</returns>
        public static NmeaType ToNmeaType(this string nmeaInput)
        {
            if (nmeaInput == null)
            {
                throw new ArgumentNullException(nameof(nmeaInput));
            }
            else if (nmeaInput.Length != 3 && nmeaInput.Length < 6)
            {
                throw new ArgumentException("rawGpsIdentifier should be exactly 3 characters or at least 6. Given: " + nmeaInput.Length, nameof(nmeaInput));
            }
            else if (nmeaInput.Length >= 6)
            {
                throw new NotImplementedException();
            }

            return nmeaInput.ToUpperInvariant() switch
            {
                "GGA" => NmeaType.GGA,
                "GLL" => NmeaType.GLL,
                "RMC" => NmeaType.RMC,
                "VTG" => NmeaType.VTG,
                "WPT" => NmeaType.WPT,
                "WPL" => NmeaType.WPT,
                _ => NmeaType.Unknown,
            };
        }
    }
}
