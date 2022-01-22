namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents an info field for position packets carrying weather information.
    /// </summary>
    public class WeatherInfo : PositionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherInfo"/> class.
        /// </summary>
        /// <param name="encodedInfoField">A string encoding of a <see cref="PositionInfo"/> with weather information.</param>
        public WeatherInfo(string encodedInfoField)
            : base(encodedInfoField)
        {
            if ((Position.SymbolTableIdentifier != '/' && Position.SymbolTableIdentifier != '\\') ||
                Position.SymbolCode != '_')
            {
                throw new ArgumentException(
                    $@"Encoded packet must have weather symbol (`/_` or `\_`). Given: `{Position.SymbolTableIdentifier}{Position.SymbolCode}",
                    nameof(encodedInfoField));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherInfo"/> class.
        /// </summary>
        /// <param name="position"><see cref="Position"/> for this packet.</param>
        /// <param name="hasMessaging">True if the sender supports messaging.</param>
        /// <param name="timestamp">Optional <see cref="Timestamp"/> for this packet.</param>
        /// <param name="comment">Optional comment for this packet.</param>
        public WeatherInfo(Position position, bool hasMessaging, Timestamp? timestamp, string? comment)
            : base(position, hasMessaging, timestamp, comment)
        {
        }

        /// <summary>
        /// Gets wind direction as degrees.
        /// </summary>
        public int? WindDirection { get; }

        /// <summary>
        /// Gets wind speed 1-minute sustained in miles per hour.
        /// </summary>
        public int? WindSpeed { get; }

        /// <summary>
        /// Gets 5-minute max wind gust in miles per hour.
        /// </summary>
        public int? WindGust { get; }

        /// <summary>
        /// Gets temperature in degrees Fahrenheit.
        /// </summary>
        public int? Temperature { get; }

        /// <summary>
        /// Gets 1-hour rainfall in 100ths of an inch.
        /// </summary>
        public int? Rainfall1Hour { get; }

        /// <summary>
        /// Gets 24-hour rainfall in 100ths of an inch.
        /// </summary>
        public int? Rainfall24Hour { get; }

        /// <summary>
        /// Gets rainfall since midnight in 100ths of an inch.
        /// </summary>
        public int? RainfallSinceMidnight { get; }

        /// <summary>
        /// Gets humidity in percentage.
        /// </summary>
        public int? Humidity { get; }

        /// <summary>
        /// Gets Barometric pressure in 10ths of millibars/10ths of hPascal.
        /// </summary>
        public int? BarometricPressure { get; }

        /// <summary>
        /// Gets luminosity in watts per square meter.
        /// </summary>
        public int? Luminosity { get; }

        /// <summary>
        /// Gets raw rain.
        /// </summary>
        public int? RainRaw { get; }

        /// <summary>
        /// Retrieves an APRS weather measurement from the comment string.
        /// </summary>
        /// <param name="element">The weather element to fetch, as defined by the ARPS specification.</param>
        /// <param name="length">The expected number of digits in the measurement.</param>
        /// <returns>An int value, if found. Else, null.</returns>
        private int? GetWeatherMeasurement(char element, int length = 3)
        {
            var match = Regex.Match(Comment, $"{element}([0-9]{{{length}}})");
            return match.Success ? int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : null;
        }
    }
}
