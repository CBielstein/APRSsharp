using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APaRSer;
using GeoCoordinatePortable;

namespace APaRSerUnitTests
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
        public void Position_Defaults()
        {
            Position p = new Position();

            Assert.AreEqual(0, p.Ambiguity);
            Assert.AreEqual('\\', p.SymbolTableIdentifier);
            Assert.AreEqual('.', p.SymbolCode);
            Assert.AreEqual(new GeoCoordinate(0, 0), p.Coordinates);
        }

        [TestMethod]
        public void Position_FromSpec()
        {
            Position p = new Position("4903.50N/07201.75W-");

            Assert.AreEqual(0, p.Ambiguity);
            Assert.AreEqual('/', p.SymbolTableIdentifier);
            Assert.AreEqual('-', p.SymbolCode);
            Assert.AreEqual(49.0583, p.Coordinates.Latitude);
            Assert.AreEqual(-72.0292, p.Coordinates.Longitude);
        }
    }
}
