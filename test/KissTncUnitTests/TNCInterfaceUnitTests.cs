namespace AprsSharpUnitTests.Protocols
{
    using System.Collections.Generic;
    using System.Text;
    using AprsSharp.Protocols.KISS;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test <see cref="TNCInterface"/> class.
    /// </summary>
    [TestClass]
    public class TNCInterfaceUnitTests
    {
        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void EncodeFrameData1()
        {
            using TNCInterface tnc = new TNCInterface();
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
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void EncodeFrameData2()
        {
            using TNCInterface tnc = new TNCInterface();
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
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void EncodeFrameDataWithEscapes()
        {
            using TNCInterface tnc = new TNCInterface();
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
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void EncodeFrameExitKissMode()
        {
            using TNCInterface tnc = new TNCInterface();
            tnc.SetTncPort(0);

            byte[] encodedBytes = tnc.ExitKISSMode();

            byte[] correctAnswer = new byte[3] { 0xC0, 0xFF, 0xC0 };

            Assert.AreEqual(correctAnswer.Length, encodedBytes.Length);

            for (int i = 0; i < correctAnswer.Length; ++i)
            {
                Assert.AreEqual(correctAnswer[i], encodedBytes[i], "At position " + i);
            }
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DataReceivedAtOnce()
        {
            using TNCInterface tnc = new TNCInterface();

            byte[] receivedData = new byte[7] { 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0 };

            byte[][] decodedFrames = tnc.DecodeReceivedData(receivedData);

            Assert.AreEqual(1, decodedFrames.Length);
            Assert.AreEqual(4, decodedFrames[0].Length);
            Assert.AreEqual("TEST", Encoding.ASCII.GetString(decodedFrames[0]));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DataReceivedSplit()
        {
            using TNCInterface tnc = new TNCInterface();

            byte[] dataRec1 = new byte[4] { 0xC0, 0x50, 0x48, 0x65 };
            byte[] dataRec2 = new byte[4] { 0x6C, 0x6C, 0x6F, 0xC0 };

            byte[][] decodedFrames = tnc.DecodeReceivedData(dataRec1);
            Assert.AreEqual(0, decodedFrames.Length);

            decodedFrames = tnc.DecodeReceivedData(dataRec2);
            Assert.AreEqual(1, decodedFrames.Length);
            Assert.AreEqual(5, decodedFrames[0].Length);
            Assert.AreEqual("Hello", Encoding.ASCII.GetString(decodedFrames[0]));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DataReceivedEscapes()
        {
            using TNCInterface tnc = new TNCInterface();

            byte[] recData = new byte[7] { 0xC0, 0x00, 0xDB, 0xDC, 0xDB, 0xDD, 0xC0 };

            byte[][] decodedFrames = tnc.DecodeReceivedData(recData);

            Assert.AreEqual(1, decodedFrames.Length);
            Assert.AreEqual(2, decodedFrames[0].Length);
            Assert.AreEqual((byte)SpecialCharacters.FEND, decodedFrames[0][0]);
            Assert.AreEqual((byte)SpecialCharacters.FESC, decodedFrames[0][1]);
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DataReceivedAtOncePrefacedMultipleFEND()
        {
            using TNCInterface tnc = new TNCInterface();

            byte[] receivedData = new byte[9] { (byte)SpecialCharacters.FEND, (byte)SpecialCharacters.FEND, 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0 };

            byte[][] decodedFrames = tnc.DecodeReceivedData(receivedData);

            Assert.AreEqual(1, decodedFrames.Length);
            Assert.AreEqual(4, decodedFrames[0].Length);
            Assert.AreEqual("TEST", Encoding.ASCII.GetString(decodedFrames[0]));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void MultipleFramesDataReceivedAtOnce()
        {
            using TNCInterface tnc = new TNCInterface();

            byte[] receivedData = new byte[15] { 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0, 0xC0, 0x50, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xC0 };

            byte[][] decodedFrames = tnc.DecodeReceivedData(receivedData);

            Assert.AreEqual(2, decodedFrames.Length);
            Assert.AreEqual(4, decodedFrames[0].Length);
            Assert.AreEqual("TEST", Encoding.ASCII.GetString(decodedFrames[0]));
            Assert.AreEqual(5, decodedFrames[1].Length);
            Assert.AreEqual("Hello", Encoding.ASCII.GetString(decodedFrames[1]));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DelegateReceivedAtOnce()
        {
            using TNCInterface tnc = new TNCInterface();
            Queue<byte[]> qDecodedFrames = new Queue<byte[]>();

            tnc.FrameReceivedEvent += (sender, arg) => qDecodedFrames.Enqueue(arg.Data);

            byte[] receivedData = new byte[7] { 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0 };

            tnc.DecodeReceivedData(receivedData);

            byte[][] decodedFrames = qDecodedFrames.ToArray();

            Assert.AreEqual(1, decodedFrames.Length);
            Assert.AreEqual(4, decodedFrames[0].Length);
            Assert.AreEqual("TEST", Encoding.ASCII.GetString(decodedFrames[0]));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DelegateDataReceivedSplit()
        {
            using TNCInterface tnc = new TNCInterface();
            Queue<byte[]> qDecodedFrames = new Queue<byte[]>();

            tnc.FrameReceivedEvent += (sender, arg) => qDecodedFrames.Enqueue(arg.Data);

            byte[] dataRec1 = new byte[4] { 0xC0, 0x50, 0x48, 0x65 };
            byte[] dataRec2 = new byte[4] { 0x6C, 0x6C, 0x6F, 0xC0 };

            tnc.DecodeReceivedData(dataRec1);

            byte[][] decodedFrames = qDecodedFrames.ToArray();

            Assert.AreEqual(0, decodedFrames.Length);

            decodedFrames = tnc.DecodeReceivedData(dataRec2);
            Assert.AreEqual(1, decodedFrames.Length);
            Assert.AreEqual(5, decodedFrames[0].Length);
            Assert.AreEqual("Hello", Encoding.ASCII.GetString(decodedFrames[0]));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DelegateDataReceivedEscapes()
        {
            using TNCInterface tnc = new TNCInterface();
            Queue<byte[]> qDecodedFrames = new Queue<byte[]>();

            tnc.FrameReceivedEvent += (sender, arg) => qDecodedFrames.Enqueue(arg.Data);

            byte[] recData = new byte[7] { 0xC0, 0x00, 0xDB, 0xDC, 0xDB, 0xDD, 0xC0 };

            tnc.DecodeReceivedData(recData);

            byte[][] decodedFrames = qDecodedFrames.ToArray();

            Assert.AreEqual(1, decodedFrames.Length);
            Assert.AreEqual(2, decodedFrames[0].Length);
            Assert.AreEqual((byte)SpecialCharacters.FEND, decodedFrames[0][0]);
            Assert.AreEqual((byte)SpecialCharacters.FESC, decodedFrames[0][1]);
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DelegateDataReceivedAtOncePrefacedMultipleFEND()
        {
            using TNCInterface tnc = new TNCInterface();
            Queue<byte[]> qDecodedFrames = new Queue<byte[]>();

            tnc.FrameReceivedEvent += (sender, arg) => qDecodedFrames.Enqueue(arg.Data);

            byte[] receivedData = new byte[9] { (byte)SpecialCharacters.FEND, (byte)SpecialCharacters.FEND, 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0 };

            tnc.DecodeReceivedData(receivedData);

            byte[][] decodedFrames = qDecodedFrames.ToArray();

            Assert.AreEqual(1, decodedFrames.Length);
            Assert.AreEqual(4, decodedFrames[0].Length);
            Assert.AreEqual("TEST", Encoding.ASCII.GetString(decodedFrames[0]));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DelegateMultipleFramesDataReceivedAtOnce()
        {
            using TNCInterface tnc = new TNCInterface();
            Queue<byte[]> qDecodedFrames = new Queue<byte[]>();

            tnc.FrameReceivedEvent += (sender, arg) => qDecodedFrames.Enqueue(arg.Data);

            byte[] receivedData = new byte[15] { 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0, 0xC0, 0x50, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xC0 };

            tnc.DecodeReceivedData(receivedData);

            byte[][] decodedFrames = qDecodedFrames.ToArray();

            Assert.AreEqual(2, decodedFrames.Length);
            Assert.AreEqual(4, decodedFrames[0].Length);
            Assert.AreEqual("TEST", Encoding.ASCII.GetString(decodedFrames[0]));
            Assert.AreEqual(5, decodedFrames[1].Length);
            Assert.AreEqual("Hello", Encoding.ASCII.GetString(decodedFrames[1]));
        }
    }
}
