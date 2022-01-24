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
        /// Verifies decoding and re-encoding a "complete weather packet" with position.
        /// These strings are taken directly from examples in the APRS 1.01 specification.
        /// </summary>
        /// <param name="encodedInfoField">The fully-encoded weather info field.</param>
        /// <param name="expectedComment">The expected comment after decode.</param>
        /// <param name="expectedBarometricPressure">The expected barometric pressure after decode.</param>
        /// <param name="expectedTemperature">The expected temperature after decode.</param>
        /// <param name="expectedPacketType">The expected <see cref="PacketType"/> after decode.</param>
        [Theory]
        [InlineData("!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900wRSW", "220/004g005t077r000p000P000h50b09900wRSW", 9900, 77, PacketType.PositionWithoutTimestampNoMessaging)]
        [InlineData("!4903.50N/07201.75W_220/004g005t077r000p000P000h50b.....wRSW", "220/004g005t077r000p000P000h50b.....wRSW", null, 77, PacketType.PositionWithoutTimestampNoMessaging)]
        [InlineData("@092345z4903.50N/07201.75W_220/004g005t-07r000p000P000h50b09900wRSW", "220/004g005t-07r000p000P000h50b09900wRSW", 9900, -7, PacketType.PositionWithTimestampWithMessaging)]
        public void TestCompleteWeatherReport(
            string encodedInfoField,
            string expectedComment,
            int? expectedBarometricPressure,
            int expectedTemperature,
            PacketType expectedPacketType)
        {
            WeatherInfo wi = new WeatherInfo(encodedInfoField);

            Assert.IsAssignableFrom<PositionInfo>(wi);
            Assert.IsType<WeatherInfo>(wi);

            bool expectedHasMessaging = wi.Type == PacketType.PositionWithoutTimestampWithMessaging || wi.Type == PacketType.PositionWithTimestampWithMessaging;
            Assert.Equal(expectedPacketType, wi.Type);
            Assert.Equal(expectedHasMessaging, wi.HasMessaging);

            Timestamp? encodeTimestamp = null;
            if (wi.Type == PacketType.PositionWithTimestampNoMessaging || wi.Type == PacketType.PositionWithTimestampWithMessaging)
            {
                Assert.NotNull(wi.Timestamp);
                Assert.Equal(TimestampType.DHMz, wi.Timestamp?.DecodedType);
                Assert.Equal(9, wi.Timestamp?.DateTime.Day);
                Assert.Equal(23, wi.Timestamp?.DateTime.Hour);
                Assert.Equal(45, wi.Timestamp?.DateTime.Minute);
                Assert.Equal(DateTimeKind.Utc, wi.Timestamp?.DateTime.Kind);

                encodeTimestamp = new Timestamp(new DateTime(2022, 1, 9, 23, 45, 0, DateTimeKind.Utc));
            }
            else
            {
                Assert.Null(wi.Timestamp);
            }

            Assert.Equal('/', wi.Position.SymbolTableIdentifier);
            Assert.Equal('_', wi.Position.SymbolCode);
            Assert.True(wi.Position.IsWeatherSymbol());

            Assert.Equal(49.0583, Math.Round(wi.Position.Coordinates.Latitude, 4));
            Assert.Equal(-72.0292, Math.Round(wi.Position.Coordinates.Longitude, 4));

            Assert.Equal(expectedComment, wi.Comment);

            Assert.Equal(220, wi.WindDirection);
            Assert.Equal(4, wi.WindSpeed);
            Assert.Equal(5, wi.WindGust);
            Assert.Equal(expectedTemperature, wi.Temperature);
            Assert.Equal(0, wi.Rainfall1Hour);
            Assert.Equal(0, wi.Rainfall24Hour);
            Assert.Equal(0, wi.RainfallSinceMidnight);
            Assert.Equal(50, wi.Humidity);
            Assert.Equal(expectedBarometricPressure, wi.BarometricPressure);

            // TODO: Un-skip other tests involving weather from the original repo.

            Assert.Equal(encodedInfoField, wi.Encode());

            // TODO Issue #105: Update this test (here and perhaps above) when the additional comment info is saved separately.
            string additionalComment = "wRSW";

            WeatherInfo encodeWi = new WeatherInfo(
                wi.Position,
                expectedHasMessaging,
                encodeTimestamp,
                additionalComment,
                220,
                4,
                5,
                expectedTemperature,
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
