namespace AprsSharp.Parsers.Aprs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
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
            Comment = $"{EncodeWeatherInfo()}{GetUserComment()}";
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
            base.Comment = $"{EncodeWeatherInfo()}{comment}";
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
        /// Gets user comment within weather data.
        /// </summary>
        public new string? Comment
        {
            get { return GetUserComment(); }
            private set { }
        }

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
            var match = Regex.Match(base.Comment, $"{measurementKey}(([0-9]{{{length}}})|(-[0-9]{{{length - 1}}}))");
            return match.Success ? int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : null;
        }

        private string? GetUserComment()
        {
            if (string.IsNullOrEmpty(base.Comment))
            {
                return null;
            }

            StringBuilder s = new ();

            // Create dictionary of measurement symbols and their lengths
            Dictionary<char, int> wxKeys = new ()
            {
                { '/', 3 },
                { 'g', 3 },
                { 't', 3 },
                { 'r', 3 },
                { 'p', 3 },
                { 'P', 3 },
                { 'h', 2 },
                { 'b', 5 },
                { 'L', 3 },
                { 'l', 3 },
                { 's', 3 },
                { '#', 3 },
            };

            // Holds the measurement and its position in the comment string for each provided measurement.
            List<WXKeyBuilder> wxKeyBuilders = new ();

            // Capture each measurement and add it to the wxKeyBuilders list.
            foreach (KeyValuePair<char, int> l in wxKeys)
                {
                    Match match = Regex.Match(base.Comment, $"{l.Key}(([0-9. ]{{{l.Value}}})|(-[0-9. ]{{{l.Value - 1}}}))");
                    if (match.Success)
                    {
                        wxKeyBuilders.Add(new WXKeyBuilder
                        {
                            WXKey = l.Key,
                            WXKeyLength = l.Value + 1,
                            WXKeyIndex = match.Groups[1].Index,
                            WXMeasurement = l.Key + base.Comment.Substring(match.Groups[1].Index, l.Value),
                        });
                    }
                }

                /*
                Now that we know which measurement keys were provided and where they are in the comment string, we rebuild the string.  This rebuilt string is what will be removed from the original comment.
                We could have removed each measurement in the foreach loop above, but if for some reason the user comment has text that matches a measurement (i.e., "MyComment is s123"),
                we don't want to remove it.

                Once the weather measurements are no longer consecutive in the original comment, i.e., index plus its length should be where the next wx key begins, we know we are in the user comment.
                */

            if (wxKeyBuilders.Count > 0)
                {
                    // Line up weather data into the order it was in the original comment.
                    wxKeyBuilders = (List<WXKeyBuilder>)wxKeyBuilders.OrderBy(x => x.WXKeyIndex).ToList();
                    int nextIndex = wxKeyBuilders[0].WXKeyIndex;

                    foreach (WXKeyBuilder w in wxKeyBuilders)
                    {
                        if (w.WXKeyIndex == nextIndex)
                        {
                            s.Append(w.WXMeasurement);
                        }
                        else
                        {
                            break;
                        }

                        nextIndex += w.WXKeyLength;
                    }

                    // Remove the weather measurements from the original comment and trim off the first three characters (wind direction).  What's left is the weather comment.
                    return base.Comment.Replace(s.ToString(), string.Empty, StringComparison.Ordinal).Remove(0, 3);
                }
                else
                {
                    return null;
                }
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
                sb.Append($"{lum}{(Luminosity % 1000).ToWeatherEncoding()}");
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

        private struct WXKeyBuilder
        {
            internal char WXKey;
            internal int WXKeyLength;
            internal int WXKeyIndex;
            internal string WXMeasurement;
        }
    }
}
