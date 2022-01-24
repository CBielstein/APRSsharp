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
        /// <param name="expectedEncodingResult">Expected result of info field encoding.
        ///     Different on some packets as APRS# code currently includes fields such as humidity even if it is null (`h..`).
        ///     This appears in line with existing software.</param>
        /// <param name="expectedComment">The expected comment after decode.</param>
        /// <param name="expectedWindDir">The expected wind direction after decode.</param>
        /// <param name="expectedWindSpeeed">The expected wind speed after decode.</param>
        /// <param name="expectedWindGust">The expected wind gust speed after decode.</param>
        /// <param name="expectedTemperature">The expected temperature after decode.</param>
        /// <param name="expectedrainfallSinceMidnight">The expected rainfall since midnight after decode.</param>
        /// <param name="expectedHumidity">The expected humidity after decode.</param>
        /// <param name="expectedBarometricPressure">The expected barometric pressure after decode.</param>
        /// <param name="additionalComment">An additional input to the comment for encode.
        ///     TODO Issue #105: Update (or at least review) this logic when the additional comment info is saved separately.</param>
        /// <param name="expectedPacketType">The expected <see cref="PacketType"/> after decode.</param>
        [Theory]
        [InlineData(
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900wRSW",
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900wRSW",
            "220/004g005t077r000p000P000h50b09900wRSW",
            220,
            4,
            5,
            77,
            0,
            50,
            9900,
            "wRSW",
            PacketType.PositionWithoutTimestampNoMessaging)]
        [InlineData(
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b.....wRSW",
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b.....wRSW",
            "220/004g005t077r000p000P000h50b.....wRSW",
            220,
            4,
            5,
            77,
            0,
            50,
            null,
            "wRSW",
            PacketType.PositionWithoutTimestampNoMessaging)]
        [InlineData(
            "@092345z4903.50N/07201.75W_220/004g005t-07r000p000P000h50b09900wRSW",
            "@092345z4903.50N/07201.75W_220/004g005t-07r000p000P000h50b09900wRSW",
            "220/004g005t-07r000p000P000h50b09900wRSW",
            220,
            4,
            5,
            -7,
            0,
            50,
            9900,
            "wRSW",
            PacketType.PositionWithTimestampWithMessaging)]
        [InlineData(
            "@092345z4903.50N/07201.75W_090/000g000t066r000p000...dUII",
            "@092345z4903.50N/07201.75W_090/000g000t066r000p000P...h..b.....dUII",
            "090/000g000t066r000p000...dUII",
            90,
            0,
            0,
            66,
            null,
            null,
            null,
            "dUII",
            PacketType.PositionWithTimestampWithMessaging)]
        public void TestCompleteWeatherReport(
            string encodedInfoField,
            string expectedEncodingResult,
            string expectedComment,
            int? expectedWindDir,
            int? expectedWindSpeeed,
            int? expectedWindGust,
            int? expectedTemperature,
            int? expectedrainfallSinceMidnight,
            int? expectedHumidity,
            int? expectedBarometricPressure,
            string additionalComment,
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

            Assert.Equal(expectedWindDir, wi.WindDirection);
            Assert.Equal(expectedWindSpeeed, wi.WindSpeed);
            Assert.Equal(expectedWindGust, wi.WindGust);
            Assert.Equal(expectedTemperature, wi.Temperature);
            Assert.Equal(0, wi.Rainfall1Hour);
            Assert.Equal(0, wi.Rainfall24Hour);
            Assert.Equal(expectedrainfallSinceMidnight, wi.RainfallSinceMidnight);
            Assert.Equal(expectedHumidity, wi.Humidity);
            Assert.Equal(expectedBarometricPressure, wi.BarometricPressure);

            Assert.Equal(encodedInfoField, wi.Encode());

            WeatherInfo encodeWi = new WeatherInfo(
                wi.Position,
                expectedHasMessaging,
                encodeTimestamp,
                additionalComment,
                expectedWindDir,
                expectedWindSpeeed,
                expectedWindGust,
                expectedTemperature,
                0,
                0,
                expectedrainfallSinceMidnight,
                expectedHumidity,
                expectedBarometricPressure,
                null,
                null,
                null);

            Assert.Equal(expectedEncodingResult, encodeWi.Encode());
        }
    }
}
