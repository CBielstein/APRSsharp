namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Test Position code.
    /// </summary>
    public class PositionUnitTests
    {
        /// <summary>
        /// Decode Latitute based on APRS spec.
        /// </summary>
        [Fact]
        public void DecodeLatitudeFromSpec()
        {
            string latitude = "4903.50N";
            Position p = new Position();

            double decodedLat = (double)p.DecodeLatitude(latitude);
            Assert.Equal(49.0583, decodedLat);
            Assert.Equal(0, p.Ambiguity);
        }

        /// <summary>
        /// Decode Latitute based on APRS spec.
        /// </summary>
        [Fact]
        public void DecodeLatitudeFromSpecNegative()
        {
            string latitude = "4903.50S";
            Position p = new Position();
            double decodedLat = (double)p.DecodeLatitude(latitude);
            Assert.Equal(-49.0583, decodedLat);
            Assert.Equal(0, p.Ambiguity);
        }

        /// <summary>
        /// Decode Latitute exception case.
        /// </summary>
        [Fact]
        public void DecodeLatitudeOutOfRange()
        {
            string latitude = "9103.50N";
            Position p = new Position();

            Assert.Throws<ArgumentOutOfRangeException>(() => p.DecodeLatitude(latitude));
        }

        /// <summary>
        /// Decode Latitute exception case.
        /// </summary>
        [Fact]
        public void DecodeLatitudeInvalidWithChars()
        {
            string latitude = "4cam.50N";
            Position p = new Position();

            Assert.Throws<FormatException>(() => p.DecodeLatitude(latitude));
        }

        /// <summary>
        /// Decode Latitute exception case.
        /// </summary>
        [Fact]
        public void DecodeLatitudeTooLong()
        {
            string latitude = "4903.500N";
            Position p = new Position();

            Assert.Throws<ArgumentException>(() => p.DecodeLatitude(latitude));
        }

        /// <summary>
        /// Decode Latitute exception case.
        /// </summary>
        [Fact]
        public void DecodeLatitudeWrongDirection()
        {
            string latitude = "4903.50E";
            Position p = new Position();

            Assert.Throws<ArgumentException>(() => p.DecodeLatitude(latitude));
        }

        /// <summary>
        /// Decode Latitute exception case.
        /// </summary>
        [Fact]
        public void DecodeLatitudeWrongDecimalPointLocation()
        {
            string latitude = "490.350N";
            Position p = new Position();

            Assert.Throws<ArgumentException>(() => p.DecodeLatitude(latitude));
        }

        /// <summary>
        /// Decode Latitute based on example from APRS spec.
        /// </summary>
        [Fact]
        public void DecodeLatitudeWithAmbiguityFromSpec1()
        {
            string latitude = "4903.5 N";
            Position p = new Position();

            double decodedLat = (double)p.DecodeLatitude(latitude);
            Assert.Equal(49.0583, decodedLat);
            Assert.Equal(1, p.Ambiguity);
        }

        /// <summary>
        /// Decode Latitute based on example from APRS spec.
        /// </summary>
        [Fact]
        public void DecodeLatitudeWithAmbiguityFromSpec2()
        {
            string latitude = "4903.  N";
            Position p = new Position();

            double decodedLat = (double)p.DecodeLatitude(latitude);
            Assert.Equal(49.05, decodedLat);
            Assert.Equal(2, p.Ambiguity);
        }

        /// <summary>
        /// Decode Latitute based on example from APRS spec.
        /// </summary>
        [Fact]
        public void DecodeLatitudeWithAmbiguityFromSpec3()
        {
            string latitude = "490 .  N";
            Position p = new Position();

            double decodedLat = (double)p.DecodeLatitude(latitude);
            Assert.Equal(49.0, decodedLat);
            Assert.Equal(3, p.Ambiguity);
        }

        /// <summary>
        /// Decode Latitute based on example from APRS spec.
        /// </summary>
        [Fact]
        public void DecodeLatitudeWithAmbiguityFromSpec4()
        {
            string latitude = "49  .  N";
            Position p = new Position();

            double decodedLat = (double)p.DecodeLatitude(latitude);
            Assert.Equal(49.0, decodedLat);
            Assert.Equal(4, p.Ambiguity);
        }

        /// <summary>
        /// Test latitute ambiguity.
        /// </summary>
        [Fact]
        public void CountAmbiguityInvalid()
        {
            string latitude = "49  .1  N";
            Assert.Throws<ArgumentException>(() => Position.CountAmbiguity(latitude));
        }

        /// <summary>
        /// Test latitute ambiguity.
        /// </summary>
        [Fact]
        public void EnforceAmbiguityBasic()
        {
            string latitude = "4903.50N";
            string ambiguous = Position.EnforceAmbiguity(latitude, 2);
            Assert.Equal("4903.  N", ambiguous);
        }

        /// <summary>
        /// Test latitute ambiguity.
        /// </summary>
        [Fact]
        public void EnforceAmbiguityBasic2()
        {
            string latitude = "4903.50N";
            string ambiguous = Position.EnforceAmbiguity(latitude, 4);
            Assert.Equal("49  .  N", ambiguous);
        }

        /// <summary>
        /// Test latitute ambiguity.
        /// </summary>
        [Fact]
        public void EnforceAmbiguityInvalidArg()
        {
            string latitude = "4903.50N";
            Assert.Throws<ArgumentOutOfRangeException>(() => Position.EnforceAmbiguity(latitude, 7));
        }

        /// <summary>
        /// Decode longitude based on example from APRS spec.
        /// </summary>
        [Fact]
        public void DecodeLongitudeFromSpec()
        {
            string longitude = "07201.75W";
            Position p = new Position();

            double decodedLong = (double)p.DecodeLongitude(longitude);
            Assert.Equal(-72.0292, decodedLong);
        }

        /// <summary>
        /// Decode longitude based on example from APRS spec.
        /// </summary>
        [Fact]
        public void DecodeLongitudeFromSpecNegative()
        {
            string longitude = "07201.75E";
            Position p = new Position();

            double decodedLong = (double)p.DecodeLongitude(longitude);
            Assert.Equal(72.0292, decodedLong);
        }

        /// <summary>
        /// Decode longitude exception case.
        /// </summary>
        [Fact]
        public void DecodeLongitudeOutOfRange()
        {
            string longitude = "18130.50E";
            Position p = new Position();

            Assert.Throws<ArgumentOutOfRangeException>(() => p.DecodeLongitude(longitude));
        }

        /// <summary>
        /// Decode longitude exception case.
        /// </summary>
        [Fact]
        public void DecodeLongitudeInvalidWithChars()
        {
            string longitude = "4cam0.50E";
            Position p = new Position();

            Assert.Throws<FormatException>(() => p.DecodeLongitude(longitude));
        }

        /// <summary>
        /// Decode longitude exception case.
        /// </summary>
        [Fact]
        public void DecodeLongitudeTooLong()
        {
            string longitude = "072010.50W";
            Position p = new Position();

            Assert.Throws<ArgumentException>(() => p.DecodeLongitude(longitude));
        }

        /// <summary>
        /// Decode longitude exception case.
        /// </summary>
        [Fact]
        public void DecodeLongitudeWrongDirection()
        {
            string longitude = "07201.50N";
            Position p = new Position();

            Assert.Throws<ArgumentException>(() => p.DecodeLongitude(longitude));
        }

        /// <summary>
        /// Decode longitude exception case.
        /// </summary>
        [Fact]
        public void DecodeLongitudeWrongDecimalPointLocation()
        {
            string longitude = "072.0175W";
            Position p = new Position();

            Assert.Throws<ArgumentException>(() => p.DecodeLongitude(longitude));
        }

        /// <summary>
        /// Decode position.
        /// </summary>
        /// <param name="encodedPosition">Encoded position to decode.</param>
        /// <param name="expectedSymbolTable">Expected decode symbol table identifier value.</param>
        /// <param name="expectedSymbolCode">Expected decode symbol code identifier value.</param>
        /// <param name="expectedLatitude">Expected decode position latidue value.</param>
        /// <param name="expectedLongitude">Expected decode position longitude value.</param>
        [Theory]
        [InlineData(null, '\\', '.', 0, 0)] // defaults
        [InlineData("4903.50N/07201.75W-", '/', '-', 49.0583, -72.0292)] // from APRS spec
        public void Decode(
            string? encodedPosition,
            char expectedSymbolTable,
            char expectedSymbolCode,
            double expectedLatitude,
            double expectedLongitude)
        {
            Position p = new Position();

            if (encodedPosition != null)
            {
                p.Decode(encodedPosition);
            }

            Assert.Equal(0, p.Ambiguity);
            Assert.Equal(expectedSymbolTable, p.SymbolTableIdentifier);
            Assert.Equal(expectedSymbolCode, p.SymbolCode);
            Assert.Equal(new GeoCoordinate(expectedLatitude, expectedLongitude), p.Coordinates);
        }

        /// <summary>
        /// Test Latitude defaults.
        /// </summary>
        [Fact]
        public void EncodeLatitudeWithDefaults()
        {
            Position p = new Position();

            string encodedLatitude = p.EncodeLatitude();

            Assert.Equal("0000.00N", encodedLatitude);
        }

        /// <summary>
        /// Encode Position based on example from APRS spec.
        /// </summary>
        /// <param name="ambiguity">The amount of ambiguity, if any, to encode.</param>
        /// <param name="expectedEncoding">The expected result of encoding.</param>
        [Theory]
        [InlineData(0, "4903.50N")]
        [InlineData(3, "490 .  N")]
        public void EncodeLatitude(int ambiguity, string expectedEncoding)
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.029);
            Position p = new Position(gc, '\\', '.', ambiguity);

            string encodedLatitude = p.EncodeLatitude();

            Assert.Equal(expectedEncoding, encodedLatitude);
        }

        /// <summary>
        /// Test encoding longitude defaults.
        /// </summary>
        [Fact]
        public void EncodeLongitudeWithDefaults()
        {
            Position p = new Position();

            string encodedLongitude = p.EncodeLongitude();

            Assert.Equal("00000.00W", encodedLongitude);
        }

        /// <summary>
        /// Encode Longitude based on example from APRS spec.
        /// </summary>
        /// <param name="ambiguity">The amount of ambiguity, if any, to encode.</param>
        /// <param name="expectedEncoding">The expected result of encoding.</param>
        [Theory]
        [InlineData(0, "07201.75W")]
        [InlineData(3, "0720 .  W")]
        public void EncodeLongitude(int ambiguity, string expectedEncoding)
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '\\', '.', ambiguity);

            string encodedLongitude = p.EncodeLongitude();

            Assert.Equal(expectedEncoding, encodedLongitude);
        }

        /// <summary>
        /// Test encoding defaults.
        /// </summary>
        [Fact]
        public void EncodeWithDefaults()
        {
            Position p = new Position();

            string encoded = p.Encode();

            Assert.Equal("0000.00N\\00000.00W.", encoded);
        }

        /// <summary>
        /// Test encoding based on example from APRS spec.
        /// </summary>
        /// <param name="ambiguity">The amount of ambiguity, if any, to encode.</param>
        /// <param name="expectedEncoding">The expected result of encoding.</param>
        [Theory]
        [InlineData(0, "4903.50N/07201.75W-")]
        [InlineData(4, "49  .  N/072  .  W-")]
        public void Encode(int ambiguity, string expectedEncoding)
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '/', '-', ambiguity);

            string encoded = p.Encode();

            Assert.Equal(expectedEncoding, encoded);
        }

        /// <summary>
        /// Test position constructor.
        /// </summary>
        [Fact]
        public void PositionConstructor()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '/', '-', 4);

            Assert.Equal(gc, p.Coordinates);
            Assert.Equal('/', p.SymbolTableIdentifier);
            Assert.Equal('-', p.SymbolCode);
            Assert.Equal(4, p.Ambiguity);
        }

        /// <summary>
        /// Test basic decoding of Maidenhead gridsquare to latidude and longitude.
        /// </summary>
        /// <param name="maidenhead">Maidenhead gridsquare to decode.</param>
        /// <param name="expectedLatitude">The expected decoded latidude value.</param>
        /// <param name="expectedLongitude">The expected encoded longitude value.</param>
        /// <param name="includesSymbol">Whether the Maidenhead to be decoded includes symbol.</param>
        [Theory]
        [InlineData("CN87uo", 47.604, -122.292, false)]
        [InlineData("EM10dg", 30.271, -97.708, false)]
        [InlineData("CN87/-", 47.5, -123.0, true)]
        [InlineData("EM10dg11/-", 30.256, -97.738, true)]
        [InlineData("PE23/-", -46.5, 125.0, true)]
        public void DecodeMaidenhead(string maidenhead, double expectedLatitude, double expectedLongitude, bool includesSymbol)
        {
            Position p = new Position();
            p.DecodeMaidenhead(maidenhead);

            Assert.Equal(expectedLatitude, Math.Round(p.Coordinates.Latitude, 3));
            Assert.Equal(expectedLongitude, Math.Round(p.Coordinates.Longitude, 3));

            if (includesSymbol)
            {
                Assert.Equal('/', p.SymbolTableIdentifier);
                Assert.Equal('-', p.SymbolCode);
            }
        }

        /// <summary>
        /// Test basic encoding of Maidenhead gridsquare to latidude and longitude.
        /// </summary>
        /// <param name="latitude">Latitude to encode.</param>
        /// <param name="longitude">Longitude to encode.</param>
        /// <param name="encodingLength">Length of Maidenhead gridsquare to produce.</param>
        /// <param name="appendSymbol">Whether or not to append the position symbol.</param>
        /// <param name="expectedMaidenhead">The expected encoded Maidenhead gridsquare.</param>
        [Theory]
        [InlineData(47.604, -122.292, 6, false, "CN87UO")]
        [InlineData(30.271, -97.708, 6, false, "EM10DG")]
        [InlineData(47.5, -123.0, 4, true, "CN87/-")]
        [InlineData(30.256, -97.738, 8, true, "EM10DG11/-")]
        [InlineData(-46.5, 125.0, 4, true, "PE23/-")]
        public void EncodeMaidenhead(
            double latitude,
            double longitude,
            int encodingLength,
            bool appendSymbol,
            string expectedMaidenhead)
        {
            Position p;
            GeoCoordinate gc = new GeoCoordinate(latitude, longitude);

            if (appendSymbol)
            {
                p = new Position(gc, '/', '-');
            }
            else
            {
                p = new Position(gc);
            }

            Assert.Equal(expectedMaidenhead, p.EncodeGridsquare(encodingLength, appendSymbol));
        }
    }
}
