namespace AprsSharpUnitTests.AprsParser
{
    using System;
    using System.Collections.Generic;
    using AprsSharp.AprsParser;
    using AprsSharp.AprsParser.Extensions;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="WeatherInfo"/> class.
    /// This is currently around position-based weather reports (APRS 1.01 spec calls this "complete weather report").
    /// </summary>
    public class WeatherInfoUnitTests
    {
        /// <summary>
        /// A standard <see cref="Timestamp"/> used throughtout these tests.
        /// </summary>
        private readonly Timestamp testTimestamp = new Timestamp(new DateTime(2022, 1, 9, 23, 45, 0, DateTimeKind.Utc)) { DecodedType = TimestampType.DHMz };

        /// <summary>
        /// A standard <see cref="Position"/> used throughout these tests.
        /// </summary>
        private readonly Position testPosition = new Position(new GeoCoordinate(49.0583, -72.0292), '/', '_');

        /// <summary>
        /// Verifies decoding and re-encoding a "complete weather packet" with position.
        /// Most of these strings are taken directly from examples in the APRS 1.01 specification. Some have been augmented with new tests.
        /// </summary>
        /// <param name="encodedInfoField">The fully-encoded weather info field.</param>
        /// <param name="expectedEncodingResult">Expected result of info field encoding.
        ///     Different on some packets as APRS# code currently includes fields such as humidity even if it is null (`h..`).
        ///     This appears in line with existing software.</param>
        /// <param name="expectedComment">The expected comment after decode.</param>
        /// <param name="expectedWindDir">The expected wind direction after decode.</param>
        /// <param name="expectedWindSpeed">The expected wind speed after decode.</param>
        /// <param name="expectedWindGust">The expected wind gust speed after decode.</param>
        /// <param name="expectedTemperature">The expected temperature after decode.</param>
        /// <param name="expectedrainfallSinceMidnight">The expected rainfall since midnight after decode.</param>
        /// <param name="expectedHumidity">The expected humidity after decode.</param>
        /// <param name="expectedBarometricPressure">The expected barometric pressure after decode.</param>
        /// <param name="expectedLuminosity">The expected luminosity after decode.</param>
        /// <param name="additionalComment">An additional input to the comment for encode.
        ///     TODO Issue #105: Update (or at least review) this logic when the additional comment info is saved separately.</param>
        /// <param name="expectedPacketType">The expected <see cref="PacketType"/> after decode.</param>
        [Theory]
        [InlineData(
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900wRSW",
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900wRSW",
            "wRSW",
            220,
            4,
            5,
            77,
            0,
            50,
            9900,
            null,
            "wRSW",
            PacketType.PositionWithoutTimestampNoMessaging)]
        [InlineData( // Luminosity 10
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900L010wRSW",
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900L010wRSW",
            "wRSW",
            220,
            4,
            5,
            77,
            0,
            50,
            9900,
            10,
            "wRSW",
            PacketType.PositionWithoutTimestampNoMessaging)]
        [InlineData( // Luminosity 1010
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900l010wRSW",
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b09900l010wRSW",
            "wRSW",
            220,
            4,
            5,
            77,
            0,
            50,
            9900,
            1010,
            "wRSW",
            PacketType.PositionWithoutTimestampNoMessaging)]
        [InlineData(
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b.....wRSW",
            "!4903.50N/07201.75W_220/004g005t077r000p000P000h50b.....wRSW",
            "wRSW",
            220,
            4,
            5,
            77,
            0,
            50,
            null,
            null,
            "wRSW",
            PacketType.PositionWithoutTimestampNoMessaging)]
        [InlineData(
            "@092345z4903.50N/07201.75W_220/004g005t-07r000p000P000h50b09900wRSW",
            "@092345z4903.50N/07201.75W_220/004g005t-07r000p000P000h50b09900wRSW",
            "wRSW",
            220,
            4,
            5,
            -7,
            0,
            50,
            9900,
            null,
            "wRSW",
            PacketType.PositionWithTimestampWithMessaging)]
        [InlineData(
            "@092345z4903.50N/07201.75W_090/000g000t066r000p000...dUII",
            "@092345z4903.50N/07201.75W_090/000g000t066r000p000P...h..b.....dUII",
            "...dUII",
            90,
            0,
            0,
            66,
            null,
            null,
            null,
            null,
            "dUII",
            PacketType.PositionWithTimestampWithMessaging)]

        // Tests if there is a measurement in the comment.  We preserve it in the comment and treat as a measurement.
        [InlineData(
            "@092345z4903.50N/07201.75W_090/000g000t066r000p000...dUIIL878",
            "@092345z4903.50N/07201.75W_090/000g000t066r000p000P...h..b.....L878...dUIIL878",
            "...dUIIL878",
            90,
            0,
            0,
            66,
            null,
            null,
            null,
            878,
            "...dUIIL878",
            PacketType.PositionWithTimestampWithMessaging)]

        // Tests if the measurement is in the data AND in the comment.  Assumes first occurrence is the measurement.
        [InlineData(
            "@092345z4903.50N/07201.75W_090/000g000t066r000p000L555dUIIL878",
            "@092345z4903.50N/07201.75W_090/000g000t066r000p000P...h..b.....L555dUIIL878",
            "dUIIL878",
            90,
            0,
            0,
            66,
            null,
            null,
            null,
            555,
            "dUIIL878",
            PacketType.PositionWithTimestampWithMessaging)]
        public void TestCompleteWeatherReport(
            string encodedInfoField,
            string expectedEncodingResult,
            string expectedComment,
            int? expectedWindDir,
            int? expectedWindSpeed,
            int? expectedWindGust,
            int? expectedTemperature,
            int? expectedrainfallSinceMidnight,
            int? expectedHumidity,
            int? expectedBarometricPressure,
            int? expectedLuminosity,
            string additionalComment,
            PacketType expectedPacketType)
        {
            WeatherInfo wi = new WeatherInfo(encodedInfoField);

            Assert.IsAssignableFrom<PositionInfo>(wi);
            Assert.IsType<WeatherInfo>(wi);

            bool expectedHasMessaging = wi.Type == PacketType.PositionWithoutTimestampWithMessaging || wi.Type == PacketType.PositionWithTimestampWithMessaging;
            Timestamp? expectedTimestamp = null;

            if (wi.Type == PacketType.PositionWithTimestampNoMessaging || wi.Type == PacketType.PositionWithTimestampWithMessaging)
            {
                expectedTimestamp = testTimestamp;
            }

            AssertWeatherInfo(
                wi,
                expectedComment,
                expectedHasMessaging,
                expectedTimestamp,
                testPosition,
                expectedWindDir,
                expectedWindSpeed,
                expectedWindGust,
                expectedTemperature,
                0,
                0,
                expectedrainfallSinceMidnight,
                expectedHumidity,
                expectedBarometricPressure,
                expectedLuminosity,
                null,
                null,
                expectedPacketType);

            Assert.Equal(encodedInfoField, wi.Encode());

            WeatherInfo encodeWi = new WeatherInfo(
                wi.Position,
                expectedHasMessaging,
                expectedTimestamp,
                additionalComment,
                expectedWindDir,
                expectedWindSpeed,
                expectedWindGust,
                expectedTemperature,
                0,
                0,
                expectedrainfallSinceMidnight,
                expectedHumidity,
                expectedBarometricPressure,
                expectedLuminosity,
                null,
                null);

            Assert.Equal(expectedEncodingResult, encodeWi.Encode());
        }

        /// <summary>
        /// Does a full TNC2 packet decode and encode on a <see cref="WeatherInfo"/> packet.
        /// </summary>
        [Fact]
        public void FullPacketRoundtrip()
        {
            string encodedPacket = @"N0CALL>WIDE1-1,WIDE2-2:/092345z4903.50N/07201.75W_180/010g015t068r001p011P010h99b09901l010#010s050 Testing WX packet.";

            Packet p = new Packet(encodedPacket);

            Assert.Equal("N0CALL", p.Sender);
            Assert.Equal(2, p.Path.Count);
            Assert.Equal("WIDE1-1", p.Path[0]);
            Assert.Equal("WIDE2-2", p.Path[1]);
            Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1), "Assert received time within the last minute.");
            Assert.Equal(PacketType.PositionWithTimestampNoMessaging, p.InfoField.Type);
            Assert.IsType<WeatherInfo>(p.InfoField);

            AssertWeatherInfo(
                p.InfoField as WeatherInfo,
                " Testing WX packet.",
                false,
                testTimestamp,
                testPosition,
                180,
                10,
                15,
                68,
                1,
                11,
                10,
                99,
                9901,
                1010,
                10,
                50,
                PacketType.PositionWithTimestampNoMessaging);

            Packet packetToEncode = new Packet(
                "N0CALL",
                new List<string>() { "WIDE1-1", "WIDE2-2" },
                new WeatherInfo(
                    testPosition,
                    false,
                    testTimestamp,
                    " Testing WX packet.",
                    180,
                    10,
                    15,
                    68,
                    1,
                    11,
                    10,
                    99,
                    9901,
                    1010,
                    10,
                    50));

            Assert.Equal(encodedPacket, p.EncodeTnc2());
            Assert.Equal(encodedPacket, packetToEncode.EncodeTnc2());
        }

        /// <summary>
        /// Asserts that a <see cref="WeatherInfo"/> has matching values for the supplied expected fields.
        /// </summary>
        private static void AssertWeatherInfo(
            WeatherInfo? actual,
            string expectedComment,
            bool expectedHasMessaging,
            Timestamp? expectedTimestamp,
            Position expectedPosition,
            int? expectedWindDir,
            int? expectedWindSpeeed,
            int? expectedWindGust,
            int? expectedTemperature,
            int? expectedrainfall1hr,
            int? expectedrainfall24hr,
            int? expectedrainfallSinceMidnight,
            int? expectedHumidity,
            int? expectedBarometricPressure,
            int? expectedLuminosity,
            int? expectedRainRaw,
            int? expectedSnow,
            PacketType expectedPacketType)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            Assert.Equal(expectedPacketType, actual.Type);
            Assert.Equal(expectedHasMessaging, actual.HasMessaging);

            if (expectedTimestamp == null)
            {
                Assert.Null(actual.Timestamp);
            }
            else
            {
                Assert.NotNull(actual.Timestamp);
                Assert.Equal(expectedTimestamp?.DecodedType, actual.Timestamp?.DecodedType);
                Assert.Equal(expectedTimestamp?.DateTime.Day, actual.Timestamp?.DateTime.Day);
                Assert.Equal(expectedTimestamp?.DateTime.Hour, actual.Timestamp?.DateTime.Hour);
                Assert.Equal(expectedTimestamp?.DateTime.Minute, actual.Timestamp?.DateTime.Minute);
                Assert.Equal(expectedTimestamp?.DateTime.Kind, actual.Timestamp?.DateTime.Kind);
            }

            Assert.Equal(expectedPosition.SymbolTableIdentifier, actual.Position.SymbolTableIdentifier);
            Assert.Equal(expectedPosition.SymbolCode, actual.Position.SymbolCode);
            Assert.True(actual.Position.IsWeatherSymbol());

            Assert.Equal(Math.Round(expectedPosition.Coordinates.Latitude, 4), Math.Round(actual.Position.Coordinates.Latitude, 4));
            Assert.Equal(Math.Round(expectedPosition.Coordinates.Longitude, 4), Math.Round(actual.Position.Coordinates.Longitude, 4));

            Assert.Equal(expectedComment, actual.Comment);

            Assert.Equal(expectedWindDir, actual.WindDirection);
            Assert.Equal(expectedWindSpeeed, actual.WindSpeed);
            Assert.Equal(expectedWindGust, actual.WindGust);
            Assert.Equal(expectedTemperature, actual.Temperature);
            Assert.Equal(expectedrainfall1hr, actual.Rainfall1Hour);
            Assert.Equal(expectedrainfall24hr, actual.Rainfall24Hour);
            Assert.Equal(expectedrainfallSinceMidnight, actual.RainfallSinceMidnight);
            Assert.Equal(expectedHumidity, actual.Humidity);
            Assert.Equal(expectedBarometricPressure, actual.BarometricPressure);
            Assert.Equal(expectedLuminosity, actual.Luminosity);
            Assert.Equal(expectedRainRaw, actual.RainRaw);
            Assert.Equal(expectedSnow, actual.Snow);
        }
    }
}
