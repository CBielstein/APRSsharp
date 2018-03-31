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
        /// Marks if the next character received should be translated as an escaped character
        /// </summary>
        private bool inEscapeMode = false;

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
            foreach (byte recByte in newBytes)
            {
                byte? byteToEnqueue = null;

                // Handle escape mode
                if (inEscapeMode)
                {
                    switch (recByte)
                    {
                        case (byte)SpecialCharacters.TFESC:
                            byteToEnqueue = (byte)SpecialCharacters.FESC;
                            break;

                        case (byte)SpecialCharacters.TFEND:
                            byteToEnqueue = (byte)SpecialCharacters.FEND;
                            break;

                        default:
                            // Not really an escape, push on the previously unused FESC character and move on
                            receivedBuffer.Enqueue((byte)SpecialCharacters.FESC);
                            break;
                    }

                    inEscapeMode = false;
                }

                // If we have already determined a byte to enqueue, no need to do this step
                if (!byteToEnqueue.HasValue)
                {
                    switch (recByte)
                    {
                        case (byte)SpecialCharacters.FEND:
                            byte[] deliverBytes = receivedBuffer.ToArray();
                            receivedBuffer.Clear();
                            DeliverBytes(deliverBytes);
                            break;

                        case (byte)SpecialCharacters.FESC:
                            inEscapeMode = true;
                            break;

                        default:
                            byteToEnqueue = recByte;
                            break;
                    }
                }

                if (byteToEnqueue.HasValue)
                {
                    receivedBuffer.Enqueue(byteToEnqueue.Value);
                }
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
