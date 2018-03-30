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
                // TODO: Handle escaped characters
                receivedBuffer.Enqueue(newByte);

                // TODO: handle arrival of a frame end for delivery
            }
        }
    }
}
