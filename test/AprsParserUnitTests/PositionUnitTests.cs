using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AprsParser;
using GeoCoordinatePortable;

namespace AprsParserUnitTests
{
    [TestClass]
    public class PositionUnitTests
    {
        [TestMethod]
        public void DecodeLatitudeFromSpec()
        {
            string latitude = "4903.50N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude);
            Assert.AreEqual(49.0583, decodedLat);
            Assert.AreEqual(0, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void DecodeLatitudeFromSpecNegative()
        {
            string latitude = "4903.50S";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude);
            Assert.AreEqual(-49.0583, decodedLat);
            Assert.AreEqual(0, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void DecodeLatitudeOutOfRange()
        {
            string latitude = "9103.50N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLatitude", latitude);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            Assert.Fail("Should have thrown an ArgumentOutOfRangeException");
        }

        [TestMethod]
        public void DecodeLatitudeInvalidWithChars()
        {
            string latitude = "4cam.50N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLatitude", latitude);
            }
            catch (FormatException)
            {
                return;
            }

            Assert.Fail("Should have thrown a FormatException");
        }

        [TestMethod]
        public void DecodeLatitudeTooLong()
        {
            string latitude = "4903.500N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLatitude", latitude);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Should have thrown an ArgumentOutOfRangeException.");
        }

        [TestMethod]
        public void DecodeLatitudeWrongDirection()
        {
            string latitude = "4903.50E";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLatitude", latitude);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Should have thrown an ArgumentException.");
        }

        [TestMethod]
        public void DecodeLatitudeWrongDecimalPointLocation()
        {
            string latitude = "490.350N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLatitude", latitude);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Should have thrown an ArgumentException.");
        }

        [TestMethod]
        public void DecodeLatitudeWithAmbiguityFromSpec1()
        {
            string latitude = "4903.5 N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude);
            Assert.AreEqual(49.0583, decodedLat);
            Assert.AreEqual(1, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void DecodeLatitudeWithAmbiguityFromSpec2()
        {
            string latitude = "4903.  N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude);
            Assert.AreEqual(49.05, decodedLat);
            Assert.AreEqual(2, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void DecodeLatitudeWithAmbiguityFromSpec3()
        {
            string latitude = "490 .  N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude);
            Assert.AreEqual(49.0, decodedLat);
            Assert.AreEqual(3, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void DecodeLatitudeWithAmbiguityFromSpec4()
        {
            string latitude = "49  .  N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude);
            Assert.AreEqual(49.0, decodedLat);
            Assert.AreEqual(4, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void CountAmbiguityInvalid()
        {
            string latitude = "49  .1  N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("CountAmbiguity", latitude);

                Assert.Fail("This should have thrown an exception.");
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("This should have thrown an ArgumentException.");
        }

        [TestMethod]
        public void EnforceAmbiguityBasic()
        {
            string latitude = "4903.50N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            string ambiguous = (string)pp.Invoke("EnforceAmbiguity", latitude, 2);
            Assert.AreEqual("4903.  N", ambiguous);
        }

        [TestMethod]
        public void EnforceAmbiguityBasic2()
        {
            string latitude = "4903.50N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            string ambiguous = (string)pp.Invoke("EnforceAmbiguity", latitude, 4);
            Assert.AreEqual("49  .  N", ambiguous);
        }

        [TestMethod]
        public void EnforceAmbiguityInvalidArg()
        {
            string latitude = "4903.50N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                string ambiguous = (string)pp.Invoke("EnforceAmbiguity", latitude, 7);

                Assert.Fail("Should have thrown an exception.");
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            Assert.Fail("Should have thrown an ArgumentOutOfRangeException.");
        }

        [TestMethod]
        public void DecodeLongitudeFromSpec()
        {
            string longitude = "07201.75W";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLong = (double)pp.Invoke("DecodeLongitude", longitude);
            Assert.AreEqual(-72.0292, decodedLong);
        }

        [TestMethod]
        public void DecodeLongitudeFromSpecNegative()
        {
            string longitude = "07201.75E";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLong = (double)pp.Invoke("DecodeLongitude", longitude);
            Assert.AreEqual(72.0292, decodedLong);
        }

        [TestMethod]
        public void DecodeLongitudeOutOfRange()
        {
            string longitude = "18130.50E";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLongitude", longitude);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            Assert.Fail("Should have thrown an ArgumentOutOfRangeException");
        }

        [TestMethod]
        public void DecodeLongitudeInvalidWithChars()
        {
            string longitude = "4cam0.50E";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLongitude", longitude);
            }
            catch (FormatException)
            {
                return;
            }

            Assert.Fail("Should have thrown a FormatException");
        }

        [TestMethod]
        public void DecodeLongitudeTooLong()
        {
            string longitude = "072010.50W";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLongitude", longitude);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Should have thrown an ArgumentException");
        }

        [TestMethod]
        public void DecodeLongitudeWrongDirection()
        {
            string longitude = "07201.50N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLongitude", longitude);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Should have thrown an ArgumentException.");
        }

        [TestMethod]
        public void DecodeLongitudeWrongDecimalPointLocation()
        {
            string longitude = "072.0175W";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("DecodeLongitude", longitude);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Should have thrown an ArgumentException.");
        }

        [TestMethod]
        public void Decode_Defaults()
        {
            Position p = new Position();

            Assert.AreEqual(0, p.Ambiguity);
            Assert.AreEqual('\\', p.SymbolTableIdentifier);
            Assert.AreEqual('.', p.SymbolCode);
            Assert.AreEqual(new GeoCoordinate(0, 0), p.Coordinates);
        }

        [TestMethod]
        public void Decode_FromSpec()
        {
            Position p = new Position();
            p.Decode("4903.50N/07201.75W-");

            Assert.AreEqual(0, p.Ambiguity);
            Assert.AreEqual('/', p.SymbolTableIdentifier);
            Assert.AreEqual('-', p.SymbolCode);
            Assert.AreEqual(49.0583, p.Coordinates.Latitude);
            Assert.AreEqual(-72.0292, p.Coordinates.Longitude);
        }

        [TestMethod]
        public void EncodeLatitude_Default()
        {
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            string encodedLatitude = (string)pp.Invoke("EncodeLatitude");

            Assert.AreEqual("0000.00N", encodedLatitude);
        }

        [TestMethod]
        public void EncodeLatitude_FromSpec()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.029);
            Position p = new Position(gc, '\\', '.', 0);
            PrivateObject pp = new PrivateObject(p);

            string encodedLatitude = (string)pp.Invoke("EncodeLatitude");

            Assert.AreEqual("4903.50N", encodedLatitude);
        }

        [TestMethod]
        public void EncodeLatitude_FromSpecWithAmbiguity()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.029);
            Position p = new Position(gc, '\\', '.', 3);
            PrivateObject pp = new PrivateObject(p);

            string encodedLatitude = (string)pp.Invoke("EncodeLatitude");

            Assert.AreEqual("490 .  N", encodedLatitude);
        }

        [TestMethod]
        public void EncodeLongitude_Default()
        {
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            string encodedLongitude = (string)pp.Invoke("EncodeLongitude");

            Assert.AreEqual("00000.00W", encodedLongitude);
        }

        [TestMethod]
        public void EncodeLongitude_FromSpec()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '\\', '.', 0);
            PrivateObject pp = new PrivateObject(p);

            string encodedLongitude = (string)pp.Invoke("EncodeLongitude");

            Assert.AreEqual("07201.75W", encodedLongitude);
        }

        [TestMethod]
        public void EncodeLongitude_FromSpecWithAmbiguity()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '\\', '.', 3);
            PrivateObject pp = new PrivateObject(p);

            string encodedLongitude = (string)pp.Invoke("EncodeLongitude");

            Assert.AreEqual("0720 .  W", encodedLongitude);
        }

        [TestMethod]
        public void Encode_Default()
        {
            Position p = new Position();

            string encoded = p.Encode();

            Assert.AreEqual("0000.00N\\00000.00W.", encoded);
        }

        [TestMethod]
        public void Encode_FromSpec()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '/', '-', 0);

            string encoded = p.Encode();

            Assert.AreEqual("4903.50N/07201.75W-", encoded);
        }

        [TestMethod]
        public void Encode_FromSpecWithAmbiguity()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '/', '-', 4);

            string encoded = p.Encode();

            Assert.AreEqual("49  .  N/072  .  W-", encoded);
        }
        
        // I had to add this one because I messed up the constructor. :(
        [TestMethod]
        public void PositionConstructor()
        {
            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            Position p = new Position(gc, '/', '-', 4);

            Assert.AreEqual(gc, p.Coordinates);
            Assert.AreEqual('/', p.SymbolTableIdentifier);
            Assert.AreEqual('-', p.SymbolCode);
            Assert.AreEqual(4, p.Ambiguity);
        }

        /// <summary>
        /// Test basic decoding of 6 char Maidenhead gridsquare to latidude and longitude
        /// </summary>
        [TestMethod]
        public void DecodeMaidenhead_1()
        {
            Position p = new Position();
            p.DecodeMaidenhead("CN87uo");

            Assert.AreEqual(47.604, Math.Round(p.Coordinates.Latitude, 3));
            Assert.AreEqual(-122.292, Math.Round(p.Coordinates.Longitude, 3));
        }

        /// <summary>
        /// Test basic decoding of 6 char Maidenhead gridsquare to latidude and longitude
        /// </summary>
        [TestMethod]
        public void DecodeMaidenhead_2()
        {
            Position p = new Position();
            p.DecodeMaidenhead("EM10dg");

            Assert.AreEqual(30.271, Math.Round(p.Coordinates.Latitude, 3));
            Assert.AreEqual(-97.708, Math.Round(p.Coordinates.Longitude, 3));
        }

        /// <summary>
        /// Test basic decoding of 4 char Maidenhead gridsquare to latidude and longitude
        /// including symbol
        /// </summary>
        [TestMethod]
        public void DecodeMaidenhead_3()
        {
            Position p = new Position();
            p.DecodeMaidenhead("CN87/-");

            Assert.AreEqual(47.5, Math.Round(p.Coordinates.Latitude, 3));
            Assert.AreEqual(-123.0, Math.Round(p.Coordinates.Longitude, 3));
            Assert.AreEqual('/', p.SymbolTableIdentifier);
            Assert.AreEqual('-', p.SymbolCode);
        }

        /// <summary>
        /// Test basic decoding of 8 char Maidenhead gridsquare to latidude and longitude, plus position symbol
        /// </summary>
        [TestMethod]
        public void DecodeMaidenhead_4()
        {
            Position p = new Position();
            p.DecodeMaidenhead("EM10dg11/-");

            Assert.AreEqual(30.256, Math.Round(p.Coordinates.Latitude, 3));
            Assert.AreEqual(-97.738, Math.Round(p.Coordinates.Longitude, 3));
            Assert.AreEqual('/', p.SymbolTableIdentifier);
            Assert.AreEqual('-', p.SymbolCode);
        }

        /// <summary>
        /// Test basic decoding of 4 char Maidenhead gridsquare to latidude and longitude
        /// including symbol
        /// </summary>
        [TestMethod]
        public void DecodeMaidenhead_5()
        {
            Position p = new Position();
            p.DecodeMaidenhead("PE23/-");

            Assert.AreEqual(-46.5, Math.Round(p.Coordinates.Latitude, 3));
            Assert.AreEqual(125.0, Math.Round(p.Coordinates.Longitude, 3));
            Assert.AreEqual('/', p.SymbolTableIdentifier);
            Assert.AreEqual('-', p.SymbolCode);
        }

        /// <summary>
        /// Test basic encoding of 6 char Maidenhead gridsquare to latidude and longitude
        /// </summary>
        [TestMethod]
        public void EncodeMaidenhead_1()
        {
            Position p = new Position(new GeoCoordinate(47.604, -122.292));
            Assert.AreEqual("CN87UO", p.EncodeGridsquare(6, false));
        }

        /// <summary>
        /// Test basic encoding of 6 char Maidenhead gridsquare to latidude and longitude
        /// </summary>
        [TestMethod]
        public void EncodeMaidenhead_2()
        {
            Position p = new Position(new GeoCoordinate(30.271, -97.708));
            Assert.AreEqual("EM10DG", p.EncodeGridsquare(6, false));
        }

        /// <summary>
        /// Test basic encoding of 4 char Maidenhead gridsquare to latidude and longitude
        /// including symbol
        /// </summary>
        [TestMethod]
        public void EncodeMaidenhead_3()
        {
            Position p = new Position(new GeoCoordinate(47.5, -123.0), '/', '-');
            Assert.AreEqual("CN87/-", p.EncodeGridsquare(4, true));
        }

        /// <summary>
        /// Test basic encoding of 8 char Maidenhead gridsquare to latidude and longitude, plus position symbol
        /// </summary>
        [TestMethod]
        public void EncodeMaidenhead_4()
        {
            Position p = new Position(new GeoCoordinate(30.256, -97.738), '/', '-');
            Assert.AreEqual("EM10DG11/-", p.EncodeGridsquare(8, true));
        }

        /// <summary>
        /// Test basic encoding of 4 char Maidenhead gridsquare to latidude and longitude
        /// including symbol
        /// </summary>
        [TestMethod]
        public void EncodeMaidenhead_5()
        {
            Position p = new Position(new GeoCoordinate(-46.5, 125.0), '/', '-');
            Assert.AreEqual("PE23/-", p.EncodeGridsquare(4, true));
        }
    }
}
