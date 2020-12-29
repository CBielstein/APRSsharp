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
        /// Decode position defaults.
        /// </summary>
        [Fact]
        public void Decode_Defaults()
        {
            Position p = new Position();

            Assert.Equal(0, p.Ambiguity);
            Assert.Equal('\\', p.SymbolTableIdentifier);
            Assert.Equal('.', p.SymbolCode);
            Assert.Equal(new GeoCoordinate(0, 0), p.Coordinates);
        }

        /// <summary>
        /// Decode Position based on example from APRS spec.
        /// </summary>
        [Fact]
        public void Decode_FromSpec()
        {
            Position p = new Position();
            p.Decode("4903.50N/07201.75W-");

            Assert.Equal(0, p.Ambiguity);
            Assert.Equal('/', p.SymbolTableIdentifier);
            Assert.Equal('-', p.SymbolCode);
            Assert.Equal(49.0583, p.Coordinates.Latitude);
            Assert.Equal(-72.0292, p.Coordinates.Longitude);
        }

        /// <summary>
        /// Test Latitude defaults.
        /// </summary>
        [Fact]
        public void EncodeLatitude_Default()
        {
            Position p = new Position();

            string encodedLatitude = p.EncodeLatitude();

            Assert.Equal("0000.00N", encodedLatitude);
        }

        /// <summary>
        /// Encode Position based on example from APRS spec.
        /// </summary>
        [Fact]
        public void EncodeLatitude_FromSpec()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.029);
            Position p = new Position(gc, '\\', '.', 0);

            string encodedLatitude = p.EncodeLatitude();

            Assert.Equal("4903.50N", encodedLatitude);
        }

        /// <summary>
        /// Encode Position based on example from APRS spec.
        /// </summary>
        [Fact]
        public void EncodeLatitude_FromSpecWithAmbiguity()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.029);
            Position p = new Position(gc, '\\', '.', 3);

            string encodedLatitude = p.EncodeLatitude();

            Assert.Equal("490 .  N", encodedLatitude);
        }

        /// <summary>
        /// Test encoding longitude defaults.
        /// </summary>
        [Fact]
        public void EncodeLongitude_Default()
        {
            Position p = new Position();

            string encodedLongitude = p.EncodeLongitude();

            Assert.Equal("00000.00W", encodedLongitude);
        }

        /// <summary>
        /// Encode Longitude based on example from APRS spec.
        /// </summary>
        [Fact]
        public void EncodeLongitude_FromSpec()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '\\', '.', 0);

            string encodedLongitude = p.EncodeLongitude();

            Assert.Equal("07201.75W", encodedLongitude);
        }

        /// <summary>
        /// Encode Longitude based on example from APRS spec.
        /// </summary>
        [Fact]
        public void EncodeLongitude_FromSpecWithAmbiguity()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '\\', '.', 3);

            string encodedLongitude = p.EncodeLongitude();

            Assert.Equal("0720 .  W", encodedLongitude);
        }

        /// <summary>
        /// Test encoding defaults.
        /// </summary>
        [Fact]
        public void Encode_Default()
        {
            Position p = new Position();

            string encoded = p.Encode();

            Assert.Equal("0000.00N\\00000.00W.", encoded);
        }

        /// <summary>
        /// Test encoding based on example from APRS spec.
        /// </summary>
        [Fact]
        public void Encode_FromSpec()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '/', '-', 0);

            string encoded = p.Encode();

            Assert.Equal("4903.50N/07201.75W-", encoded);
        }

        /// <summary>
        /// Encode based on example from APRS spec.
        /// </summary>
        [Fact]
        public void Encode_FromSpecWithAmbiguity()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '/', '-', 4);

            string encoded = p.Encode();

            Assert.Equal("49  .  N/072  .  W-", encoded);
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
        /// Test basic decoding of 6 char Maidenhead gridsquare to latidude and longitude.
        /// </summary>
        [Fact]
        public void DecodeMaidenhead_1()
        {
            Position p = new Position();
            p.DecodeMaidenhead("CN87uo");

            Assert.Equal(47.604, Math.Round(p.Coordinates.Latitude, 3));
            Assert.Equal(-122.292, Math.Round(p.Coordinates.Longitude, 3));
        }

        /// <summary>
        /// Test basic decoding of 6 char Maidenhead gridsquare to latidude and longitude.
        /// </summary>
        [Fact]
        public void DecodeMaidenhead_2()
        {
            Position p = new Position();
            p.DecodeMaidenhead("EM10dg");

            Assert.Equal(30.271, Math.Round(p.Coordinates.Latitude, 3));
            Assert.Equal(-97.708, Math.Round(p.Coordinates.Longitude, 3));
        }

        /// <summary>
        /// Test basic decoding of 4 char Maidenhead gridsquare to latidude and longitude
        /// including symbol.
        /// </summary>
        [Fact]
        public void DecodeMaidenhead_3()
        {
            Position p = new Position();
            p.DecodeMaidenhead("CN87/-");

            Assert.Equal(47.5, Math.Round(p.Coordinates.Latitude, 3));
            Assert.Equal(-123.0, Math.Round(p.Coordinates.Longitude, 3));
            Assert.Equal('/', p.SymbolTableIdentifier);
            Assert.Equal('-', p.SymbolCode);
        }

        /// <summary>
        /// Test basic decoding of 8 char Maidenhead gridsquare to latidude and longitude, plus position symbol.
        /// </summary>
        [Fact]
        public void DecodeMaidenhead_4()
        {
            Position p = new Position();
            p.DecodeMaidenhead("EM10dg11/-");

            Assert.Equal(30.256, Math.Round(p.Coordinates.Latitude, 3));
            Assert.Equal(-97.738, Math.Round(p.Coordinates.Longitude, 3));
            Assert.Equal('/', p.SymbolTableIdentifier);
            Assert.Equal('-', p.SymbolCode);
        }

        /// <summary>
        /// Test basic decoding of 4 char Maidenhead gridsquare to latidude and longitude
        /// including symbol.
        /// </summary>
        [Fact]
        public void DecodeMaidenhead_5()
        {
            Position p = new Position();
            p.DecodeMaidenhead("PE23/-");

            Assert.Equal(-46.5, Math.Round(p.Coordinates.Latitude, 3));
            Assert.Equal(125.0, Math.Round(p.Coordinates.Longitude, 3));
            Assert.Equal('/', p.SymbolTableIdentifier);
            Assert.Equal('-', p.SymbolCode);
        }

        /// <summary>
        /// Test basic encoding of 6 char Maidenhead gridsquare to latidude and longitude.
        /// </summary>
        [Fact]
        public void EncodeMaidenhead_1()
        {
            Position p = new Position(new GeoCoordinate(47.604, -122.292));
            Assert.Equal("CN87UO", p.EncodeGridsquare(6, false));
        }

        /// <summary>
        /// Test basic encoding of 6 char Maidenhead gridsquare to latidude and longitude.
        /// </summary>
        [Fact]
        public void EncodeMaidenhead_2()
        {
            Position p = new Position(new GeoCoordinate(30.271, -97.708));
            Assert.Equal("EM10DG", p.EncodeGridsquare(6, false));
        }

        /// <summary>
        /// Test basic encoding of 4 char Maidenhead gridsquare to latidude and longitude
        /// including symbol.
        /// </summary>
        [Fact]
        public void EncodeMaidenhead_3()
        {
            Position p = new Position(new GeoCoordinate(47.5, -123.0), '/', '-');
            Assert.Equal("CN87/-", p.EncodeGridsquare(4, true));
        }

        /// <summary>
        /// Test basic encoding of 8 char Maidenhead gridsquare to latidude and longitude, plus position symbol.
        /// </summary>
        [Fact]
        public void EncodeMaidenhead_4()
        {
            Position p = new Position(new GeoCoordinate(30.256, -97.738), '/', '-');
            Assert.Equal("EM10DG11/-", p.EncodeGridsquare(8, true));
        }

        /// <summary>
        /// Test basic encoding of 4 char Maidenhead gridsquare to latidude and longitude
        /// including symbol.
        /// </summary>
        [Fact]
        public void EncodeMaidenhead_5()
        {
            Position p = new Position(new GeoCoordinate(-46.5, 125.0), '/', '-');
            Assert.Equal("PE23/-", p.EncodeGridsquare(4, true));
        }
    }
}
