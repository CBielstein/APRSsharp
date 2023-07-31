namespace AprsSharp.AprsParser.Extensions
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Extension methods for handling weather packets.
    /// </summary>
    public static class WeatherExtensions
    {
        /// <summary>
        /// Determines if a <see cref="Position"/>'s symbol is a weather station.
        /// </summary>
        /// <param name="position">A <see cref="Position"/> to check.</param>
        /// <returns>True if the <see cref="Position"/>'s symbol is a weather station, else false.</returns>
        public static bool IsWeatherSymbol(this Position position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            return (position.SymbolTableIdentifier == '/' || position.SymbolTableIdentifier == '\\') && position.SymbolCode == '_';
        }

        /// <summary>
        /// Returns a string encoding of a single measurement for a complete weather packet.
        /// </summary>
        /// <param name="measurement">The measurement to convert to string.</param>
        /// <param name="encodingLength">The number of characters to use in this representation.</param>
        /// <returns>The integer as a string or all dots if null.</returns>
        public static string ToWeatherEncoding(this int? measurement, int encodingLength = 3)
        {
            if (measurement == null)
            {
                return new string('.', encodingLength);
            }

            string encoded = Math.Abs(measurement.Value).ToString(CultureInfo.InvariantCulture).PadLeft(encodingLength, '0');

            // If negative, ensure the negative sign is at the front of the padding zeros instead of in the middle
            // that's why we take the absolute value above, so that we can put the negative sign all the way at the front.
            if (measurement < 0)
            {
                encoded = $"-{encoded.Substring(1)}";
            }

            return encoded;
        }
    }
}
