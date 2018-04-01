using Microsoft.VisualStudio.TestTools.UnitTesting;
using KissIt;
using System.Text;
using System;

namespace KissItUnitTests
{
    [TestClass]
    public class TNCInterfaceUnitTests
    {
        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format
        /// </summary>
        [TestMethod]
        public void EncodeFrameData1()
        {
            TNCInterface tnc = new TNCInterface();
            tnc.SetTncPort(0);

            string message = "TEST";

            byte[] encodedBytes = tnc.SendData(Encoding.ASCII.GetBytes(message));

            byte[] correctAnswer = new byte[7] { 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0 };

            Assert.AreEqual(correctAnswer.Length, encodedBytes.Length);

            for (int i = 0; i < correctAnswer.Length; ++i)
            {
                Assert.AreEqual(correctAnswer[i], encodedBytes[i], "At position " + i);
            }

            Assert.IsTrue(true);
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format
        /// </summary>
        [TestMethod]
        public void EncodeFrameData2()
        {
            TNCInterface tnc = new TNCInterface();
            tnc.SetTncPort(5);

            string message = "Hello";

            byte[] encodedBytes = tnc.SendData(Encoding.ASCII.GetBytes(message));

            byte[] correctAnswer = new byte[8] { 0xC0, 0x50, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xC0 };

            Assert.AreEqual(correctAnswer.Length, encodedBytes.Length);

            for (int i = 0; i < correctAnswer.Length; ++i)
            {
                Assert.AreEqual(correctAnswer[i], encodedBytes[i], "At position " + i);
            }
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format
        /// </summary>
        [TestMethod]
        public void EncodeFrameDataWithEscapes()
        {
            TNCInterface tnc = new TNCInterface();
            tnc.SetTncPort(0);

            byte[] data = new byte[2] { (byte)SpecialCharacters.FEND, (byte)SpecialCharacters.FESC };

            byte[] encodedBytes = tnc.SendData(data);

            byte[] correctAnswer = new byte[7] { 0xC0, 0x00, 0xDB, 0xDC, 0xDB, 0xDD, 0xC0 };

            Assert.AreEqual(correctAnswer.Length, encodedBytes.Length);

            for (int i = 0; i < correctAnswer.Length; ++i)
            {
                Assert.AreEqual(correctAnswer[i], encodedBytes[i], "At position " + i);
            }
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format
        /// </summary>
        [TestMethod]
        public void EncodeFrameExitKissMode()
        {
            TNCInterface tnc = new TNCInterface();
            tnc.SetTncPort(0);

            byte[] encodedBytes = tnc.ExitKISSMode();

            byte[] correctAnswer = new byte[3] { 0xC0, 0xFF, 0xC0 };

            Assert.AreEqual(correctAnswer.Length, encodedBytes.Length);

            for (int i = 0; i < correctAnswer.Length; ++i)
            {
                Assert.AreEqual(correctAnswer[i], encodedBytes[i], "At position " + i);
            }
        }
    }
}
