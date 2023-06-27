namespace AprsSharpUnitTests.Protocols
{
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Text;
    using AprsSharp.Protocols.KISS;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test <see cref="SerialTNC"/> class.
    /// </summary>
    [TestClass]
    public class SerialTNCUnitTests
    {
        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void EncodeFrameData1()
        {
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);

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
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);
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
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);
            tnc.SetTncPort(0);

            byte[] data = new byte[2] { (byte)SpecialCharacter.FEND, (byte)SpecialCharacter.FESC };

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
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);
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
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);

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
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);

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
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);

            byte[] recData = new byte[7] { 0xC0, 0x00, 0xDB, 0xDC, 0xDB, 0xDD, 0xC0 };

            byte[][] decodedFrames = tnc.DecodeReceivedData(recData);

            Assert.AreEqual(1, decodedFrames.Length);
            Assert.AreEqual(2, decodedFrames[0].Length);
            Assert.AreEqual((byte)SpecialCharacter.FEND, decodedFrames[0][0]);
            Assert.AreEqual((byte)SpecialCharacter.FESC, decodedFrames[0][1]);
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DataReceivedAtOncePrefacedMultipleFEND()
        {
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);

            byte[] receivedData = new byte[9] { (byte)SpecialCharacter.FEND, (byte)SpecialCharacter.FEND, 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0 };

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
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);

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
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);
            Queue<IReadOnlyList<byte>> decodedFrames = new Queue<IReadOnlyList<byte>>();

            tnc.FrameReceivedEvent += (sender, arg) => decodedFrames.Enqueue(arg.Data);

            byte[] receivedData = new byte[7] { 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0 };

            tnc.DecodeReceivedData(receivedData);

            Assert.AreEqual(1, decodedFrames.Count);

            IReadOnlyList<byte> frame = decodedFrames.Dequeue();
            Assert.AreEqual(4, frame.Count);
            Assert.AreEqual("TEST", Encoding.ASCII.GetString(frame.ToArray()));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DelegateDataReceivedSplit()
        {
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);
            Queue<IReadOnlyList<byte>> decodedFrames = new Queue<IReadOnlyList<byte>>();

            tnc.FrameReceivedEvent += (sender, arg) => decodedFrames.Enqueue(arg.Data);

            byte[] dataRec1 = new byte[4] { 0xC0, 0x50, 0x48, 0x65 };
            byte[] dataRec2 = new byte[4] { 0x6C, 0x6C, 0x6F, 0xC0 };

            tnc.DecodeReceivedData(dataRec1);

            Assert.AreEqual(0, decodedFrames.Count);

            tnc.DecodeReceivedData(dataRec2);

            Assert.AreEqual(1, decodedFrames.Count);

            IReadOnlyList<byte> frame = decodedFrames.Dequeue();
            Assert.AreEqual(5, frame.Count);
            Assert.AreEqual("Hello", Encoding.ASCII.GetString(frame.ToArray()));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DelegateDataReceivedEscapes()
        {
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);
            Queue<IReadOnlyList<byte>> decodedFrames = new Queue<IReadOnlyList<byte>>();

            tnc.FrameReceivedEvent += (sender, arg) => decodedFrames.Enqueue(arg.Data);

            byte[] recData = new byte[7] { 0xC0, 0x00, 0xDB, 0xDC, 0xDB, 0xDD, 0xC0 };

            tnc.DecodeReceivedData(recData);

            Assert.AreEqual(1, decodedFrames.Count);

            IReadOnlyList<byte> frame = decodedFrames.Dequeue();
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual((byte)SpecialCharacter.FEND, frame[0]);
            Assert.AreEqual((byte)SpecialCharacter.FESC, frame[1]);
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DelegateDataReceivedAtOncePrefacedMultipleFEND()
        {
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);
            Queue<IReadOnlyList<byte>> decodedFrames = new Queue<IReadOnlyList<byte>>();

            tnc.FrameReceivedEvent += (sender, arg) => decodedFrames.Enqueue(arg.Data);

            byte[] receivedData = new byte[9] { (byte)SpecialCharacter.FEND, (byte)SpecialCharacter.FEND, 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0 };

            tnc.DecodeReceivedData(receivedData);

            Assert.AreEqual(1, decodedFrames.Count);

            IReadOnlyList<byte> frame = decodedFrames.Dequeue();
            Assert.AreEqual(4, frame.Count);
            Assert.AreEqual("TEST", Encoding.ASCII.GetString(frame.ToArray()));
        }

        /// <summary>
        /// Test sample taken from https://en.wikipedia.org/wiki/KISS_(TNC)#Packet_format.
        /// </summary>
        [TestMethod]
        public void DelegateMultipleFramesDataReceivedAtOnce()
        {
            var mockPort = GetMockSerialPort();
            using SerialTNC tnc = new SerialTNC(mockPort.Object, 0);
            Queue<IReadOnlyList<byte>> decodedFrames = new Queue<IReadOnlyList<byte>>();

            tnc.FrameReceivedEvent += (sender, arg) => decodedFrames.Enqueue(arg.Data);

            byte[] receivedData = new byte[15] { 0xC0, 0x00, 0x54, 0x45, 0x53, 0x54, 0xC0, 0xC0, 0x50, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xC0 };

            tnc.DecodeReceivedData(receivedData);

            Assert.AreEqual(2, decodedFrames.Count);

            IReadOnlyList<byte> frame = decodedFrames.Dequeue();
            Assert.AreEqual(4, frame.Count);
            Assert.AreEqual("TEST", Encoding.ASCII.GetString(frame.ToArray()));

            frame = decodedFrames.Dequeue();
            Assert.AreEqual(5, frame.Count);
            Assert.AreEqual("Hello", Encoding.ASCII.GetString(frame.ToArray()));
        }

        /// <summary>
        /// Configures a <see cref="Mock"/> of <see cref="SerialPort"/> to use in tests.
        /// </summary>
        /// <returns>A mocked <see cref="SerialPort"/>.</returns>
        private static Mock<ISerialConnection> GetMockSerialPort()
        {
            var mockPort = new Mock<ISerialConnection>();
            mockPort.SetupGet(mock => mock.IsOpen).Returns(false);
            mockPort.Setup(mock => mock.Open());
            mockPort.Setup(mock => mock.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()));
            return mockPort;
        }
    }
}
