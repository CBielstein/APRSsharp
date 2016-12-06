using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APaRSer;

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

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude, 0);
            Assert.AreEqual(49.0583, decodedLat);
            Assert.AreEqual(0, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void DecodeLatitudeWithAmbiguityFromSpec1()
        {
            string latitude = "4903.5 N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude, 0);
            Assert.AreEqual(49.0583, decodedLat);
            Assert.AreEqual(1, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void DecodeLatitudeWithAmbiguityFromSpec2()
        {
            string latitude = "4903.  N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude, 0);
            Assert.AreEqual(49.05, decodedLat);
            Assert.AreEqual(2, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void DecodeLatitudeWithAmbiguityFromSpec3()
        {
            string latitude = "490 .  N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude, 0);
            Assert.AreEqual(49.0, decodedLat);
            Assert.AreEqual(3, pp.GetField("Ambiguity"));
        }

        [TestMethod]
        public void DecodeLatitudeWithAmbiguityFromSpec4()
        {
            string latitude = "49  .  N";
            Position p = new Position();
            PrivateObject pp = new PrivateObject(p);

            double decodedLat = (double)pp.Invoke("DecodeLatitude", latitude, 0);
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
    }
}
