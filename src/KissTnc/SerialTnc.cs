namespace AprsSharp.KissTnc
{
    using System;
    using System.IO.Ports;

    /// <summary>
    /// Represents an interface through a serial connection to a TNC using the KISS protocol.
    /// </summary>
    public sealed class SerialTNC : TNCInterface
    {
        /// <summary>
        /// The serial port to which the TNC is connected.
        /// </summary>
        private readonly ISerialConnection serialPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialTNC"/> class.
        /// </summary>
        /// <param name="serialPort">The SerialPort to use for connection to the TNC.</param>
        /// <param name="tncPort">The port on the TNC used for communication.</param>
        public SerialTNC(ISerialConnection serialPort, byte tncPort)
            : base(tncPort)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }
            else if (serialPort.IsOpen)
            {
                throw new ArgumentException("Port should not yet be opened", nameof(serialPort));
            }

            this.serialPort = serialPort;
            this.serialPort.Open();
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(HandleDataFromSerialPort);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected override void SendToTnc(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            serialPort.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Handle the DataReceived event from the serialPort.
        /// </summary>
        /// <param name="sender">SerialPort which has received data.</param>
        /// <param name="args">Additional args.</param>
        private void HandleDataFromSerialPort(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort sp = (SerialPort)sender;

            int numBytesToRead = sp.BytesToRead;
            byte[] bytesReceived = new byte[numBytesToRead];
            int numBytesWereRead = sp.Read(bytesReceived, 0, numBytesToRead);

            if (numBytesWereRead < numBytesToRead)
            {
                Array.Resize(ref bytesReceived, numBytesWereRead);
            }

            DecodeReceivedData(bytesReceived);
        }
    }
}
