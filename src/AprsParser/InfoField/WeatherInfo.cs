namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using AprsSharp.Parsers.Aprs.Extensions;

    /// <summary>
    /// Represents an info field for position packets carrying weather information.
    /// </summary>
    public class WeatherInfo : PositionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherInfo"/> class from an encoded <see cref="InfoField"/>.
        /// </summary>
        /// <param name="encodedInfoField">A string encoding of a <see cref="PositionInfo"/> with weather information.</param>
        public WeatherInfo(string encodedInfoField)
            : this(new PositionInfo(encodedInfoField))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherInfo"/> class from a <see cref="PositionInfo"/>.
        /// </summary>
        /// <param name="positionInfo">A <see cref="PositionInfo"/> from which to decode a <see cref="WeatherInfo"/>.</param>
        public WeatherInfo(PositionInfo positionInfo)
            : base(positionInfo)
        {
            if (positionInfo == null)
            {
                throw new ArgumentNullException(nameof(positionInfo));
            }

            if (!positionInfo.Position.IsWeatherSymbol())
            {
                throw new ArgumentException(
                    $@"Encoded packet must have weather symbol (`/_` or `\_`). Given: `{positionInfo.Position.SymbolTableIdentifier}{positionInfo.Position.SymbolCode}",
                    nameof(positionInfo));
            }

            WindDirection = GetWeatherMeasurement('^');
            WindSpeed = GetWeatherMeasurement('/');
            WindGust = GetWeatherMeasurement('g');
            Temperature = GetWeatherMeasurement('t');
            Rainfall1Hour = GetWeatherMeasurement('r');
            Rainfall24Hour = GetWeatherMeasurement('p');
            RainfallSinceMidnight = GetWeatherMeasurement('P');
            Humidity = GetWeatherMeasurement('h', 2);
            BarometricPressure = GetWeatherMeasurement('b', 5);
            Luminosity = GetWeatherMeasurement('L') ?? GetWeatherMeasurement('l') + 1000;
            RainRaw = GetWeatherMeasurement('#');
            Snow = GetWeatherMeasurement('s');
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherInfo"/> class.
        /// </summary>
        /// <param name="position"><see cref="Position"/> for this packet.</param>
        /// <param name="hasMessaging">True if the sender supports messaging.</param>
        /// <param name="timestamp">Optional <see cref="Timestamp"/> for this packet.</param>
        /// <param name="comment">Optional comment for this packet, will be appended after encoded weather information.</param>
        /// <param name="windDirection">Wind direction in degrees.</param>
        /// <param name="windSpeed">Wind speed 1-minute sustained in miles per hour.</param>
        /// <param name="windGust">5-minute max wind gust in miles per hour.</param>
        /// <param name="temperature">Temperature in degrees Fahrenheit.</param>
        /// <param name="rainfall1Hour">1-hour rainfall in 100ths of an inch.</param>
        /// <param name="rainfall24Hour">24-hour rainfall in 100ths of an inch.</param>
        /// <param name="rainfallSinceMidnight">Rainfall since midnight in 100ths of an inch.</param>
        /// <param name="humidity">Humidity in percentage.</param>
        /// <param name="barometricPressure">Barometric pressure in 10ths of millibars/10ths of hPascal.</param>
        /// <param name="luminosity">Luminosity in watts per square meter.</param>
        /// <param name="rainRaw">Raw rain.</param>
        /// <param name="snow">Snowfall in inches in the last 24 hours.</param>
        public WeatherInfo(
            Position position,
            bool hasMessaging,
            Timestamp? timestamp,
            string? comment,
            int? windDirection,
            int? windSpeed,
            int? windGust,
            int? temperature,
            int? rainfall1Hour,
            int? rainfall24Hour,
            int? rainfallSinceMidnight,
            int? humidity,
            int? barometricPressure,
            int? luminosity,
            int? rainRaw,
            int? snow)
            : base(position, hasMessaging, timestamp, comment)
        {
            WindDirection = windDirection;
            WindSpeed = windSpeed;
            WindGust = windGust;
            Temperature = temperature;
            Rainfall1Hour = rainfall1Hour;
            Rainfall24Hour = rainfall24Hour;
            RainfallSinceMidnight = rainfallSinceMidnight;
            Humidity = humidity;
            BarometricPressure = barometricPressure;
            Luminosity = luminosity;
            RainRaw = rainRaw;
            Snow = snow;

            Comment = $"{EncodeWeatherInfo()}{comment}";
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
        /// Gets snowfall in inches in the last 24 hours.
        /// </summary>
        public int? Snow { get; }

        /// <summary>
        /// Retrieves an APRS weather measurement from the comment string.
        /// </summary>
        /// <param name="measurementKey">The weather element to fetch, as defined by the key the ARPS specification.</param>
        /// <param name="length">The expected number of digits in the measurement.</param>
        /// <returns>An int value, if found. Else, null.</returns>
        private int? GetWeatherMeasurement(char measurementKey, int length = 3)
        {
            // Regex below looks for the measurement key followed by either `length` numbers
            // or a negative number of `length - 1` digits (to allow for the negative sign)
            var match = Regex.Match(Comment, $"{measurementKey}(([0-9]{{{length}}})|(-[0-9]{{{length - 1}}}))");
            return match.Success ? int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Encodes weather information to be placed in the comment field.
        /// </summary>
        /// <returns>An APRS encoding of weather information on this packet.</returns>
        private string EncodeWeatherInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(WindDirection.ToWeatherEncoding());
            sb.Append($"/{WindSpeed.ToWeatherEncoding()}");
            sb.Append($"g{WindGust.ToWeatherEncoding()}");
            sb.Append($"t{Temperature.ToWeatherEncoding()}");
            sb.Append($"r{Rainfall1Hour.ToWeatherEncoding()}");
            sb.Append($"p{Rainfall24Hour.ToWeatherEncoding()}");
            sb.Append($"P{RainfallSinceMidnight.ToWeatherEncoding()}");
            sb.Append($"h{Humidity.ToWeatherEncoding(2)}");
            sb.Append($"b{BarometricPressure.ToWeatherEncoding(5)}");

            // Only add less common measurements if provided
            if (Luminosity != null)
            {
                char lum = Luminosity < 1000 ? 'L' : 'l';
                sb.Append($"{lum}{Luminosity % 1000}");
            }

            if (RainRaw != null)
            {
                sb.Append($"#{RainRaw.ToWeatherEncoding()}");
            }

            if (Snow != null)
            {
                sb.Append($"s{Snow.ToWeatherEncoding()}");
            }

            return sb.ToString();
        }
    }
}
