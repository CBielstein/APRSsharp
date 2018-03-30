using System.IO.Ports;
using System.Collections.Generic;
using System.Text;
using System;

namespace KissIt
{
    /// <summary>
    /// Represents an interface through a serial connection to a TNC using the KISS protocol
    /// </summary>
    public class TNCInterface
    {
        /// <summary>
        /// The serial port to which the TNC is connected
        /// </summary>
        private SerialPort serialPort;

        /// <summary>
        /// A queue of received bytes waiting to be delivered (at the end of a frame)
        /// </summary>
        private Queue<byte> receivedBuffer;

        /// <summary>
        /// Initializes the TNCInterface with a serial port
        /// </summary>
        /// <param name="serialPortName">The name of the SerialPort to use</param>
        public TNCInterface(string serialPortName)
        {
            serialPort = new SerialPort(serialPortName);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(TNCDataReceivedEventHandler);
            receivedBuffer = new Queue<byte>();
        }

        /// <summary>
        /// Sends bytes on the SerialPort
        /// </summary>
        /// <param name="bytes">Bytes to send</param>
        public void Send(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException();
            }
            else if (bytes.Length == 0)
            {
                throw new ArgumentException("Bytes to send has length zero.");
            }

            byte[] encodedBytes = EncodeBytes(bytes);

            serialPort.Write(encodedBytes, 0, encodedBytes.Length);
        }

        /// <summary>
        /// Encodes bytes for KISS protocol
        /// </summary>
        /// <param name="bytes">Bytes to encode</param>
        /// <returns>Encoded bytes</returns>
        private byte[] EncodeBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle the DataReceived event from the serialPort
        /// </summary>
        /// <param name="sender">SerialPort which has received data</param>
        /// <param name="args">Additional args</param>
        private void TNCDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort sp = (SerialPort)sender;

            int numBytesToRead = sp.BytesToRead;
            byte[] bytesReceived = new byte[numBytesToRead];
            int numBytesWereRead = sp.Read(bytesReceived, 0, numBytesToRead);

            if (numBytesWereRead < numBytesToRead)
            {
                Array.Resize(ref bytesReceived, numBytesWereRead);
            }

            ReceivedData(bytesReceived);
        }

        /// <summary>
        /// Adds bytes to the queue, dequeues a complete frame if available
        /// </summary>
        /// <param name="newBytes">Bytes which have just arrived</param>
        private void ReceivedData(byte[] newBytes)
        {
            foreach (byte newByte in newBytes)
            {
                if (newByte == (byte)KISSSpecialCharacters.FEND)
                {
                    byte[] deliverBytes = receivedBuffer.ToArray();
                    receivedBuffer.Clear();
                    DeliverBytes(deliverBytes);
                    continue;
                }
                else if (newByte == (byte)KISSSpecialCharacters.FESC ||
                         newByte == (byte)KISSSpecialCharacters.TFEND ||
                         newByte == (byte)KISSSpecialCharacters.TFESC)
                {
                    // TODO: Handle escaped characters
                    throw new NotImplementedException();
                }

                receivedBuffer.Enqueue(newByte);
            }
        }

        /// <summary>
        /// Handles delivery of bytes. Called when the contents of the buffer are ready to be delivered
        /// </summary>
        /// <param name="bytes">Bytes to be delivered</param>
        private void DeliverBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            // TODO: Raise event with bytes received
            throw new NotImplementedException();
        }
    }
}
