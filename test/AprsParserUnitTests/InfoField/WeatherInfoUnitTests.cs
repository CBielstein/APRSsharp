namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using AprsSharp.Parsers.Aprs;
    using AprsSharp.Parsers.Aprs.Extensions;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="WeatherInfo"/> class.
    /// This is currently around position-based weather reports (APRS 1.01 spec calls this "complete weather report").
    /// </summary>
    public class WeatherInfoUnitTests
    {
        /// <summary>
        /// Verifies decoding and re-encoding a "complete weather packet" with position but without timestamp.
        /// </summary>
        /// <param name="encodedInfoField">The fully-encoded weather info field.</param>
        /// <param name="expectedComment">The expected comment after decode.</param>
        /// <param name="expectedBarometricPressure">The expected barometric pressure after decode.</param>
        [Theory]
        [InlineData("!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900wRSW", "220/004g005t077r000p000P000h50b09900wRSW", 9900)]
        [InlineData("!4903.50N/07201.75W_220/004g005t077r000p000P000h50b.....wRSW", "220/004g005t077r000p000P000h50b.....wRSW", null)]
        public void TestCompletePositionNoTimestamp(string encodedInfoField, string expectedComment, int? expectedBarometricPressure)
        {
            WeatherInfo wi = new WeatherInfo(encodedInfoField);

            Assert.IsAssignableFrom<PositionInfo>(wi);
            Assert.IsType<WeatherInfo>(wi);

            Assert.Equal(PacketType.PositionWithoutTimestampNoMessaging, wi.Type);
            Assert.False(wi.HasMessaging);

            Assert.Equal('/', wi.Position.SymbolTableIdentifier);
            Assert.Equal('_', wi.Position.SymbolCode);
            Assert.True(wi.Position.IsWeatherSymbol());

            Assert.Equal(49.0583, Math.Round(wi.Position.Coordinates.Latitude, 4));
            Assert.Equal(-72.0292, Math.Round(wi.Position.Coordinates.Longitude, 4));

            Assert.Equal(expectedComment, wi.Comment);

            Assert.Equal(220, wi.WindDirection);
            Assert.Equal(4, wi.WindSpeed);
            Assert.Equal(5, wi.WindGust);
            Assert.Equal(77, wi.Temperature);
            Assert.Equal(0, wi.Rainfall1Hour);
            Assert.Equal(0, wi.Rainfall24Hour);
            Assert.Equal(0, wi.RainfallSinceMidnight);
            Assert.Equal(50, wi.Humidity);
            Assert.Equal(expectedBarometricPressure, wi.BarometricPressure);

            // TODO: Assert APRS software
            // TODO: Assert WX unit
            // TODO: Encode new object
            // TODO: Reuse the InfoField constructor on other types to reduce code duplication?

            Assert.Equal(encodedInfoField, wi.Encode());

            WeatherInfo encodeWi = new WeatherInfo(
                wi.Position,
                false,
                null,
                null,
                220,
                4,
                5,
                77,
                0,
                0,
                0,
                50,
                expectedBarometricPressure,
                null,
                null,
                null);

            Assert.Equal(encodedInfoField, encodeWi.Encode());
        }
    }
}
