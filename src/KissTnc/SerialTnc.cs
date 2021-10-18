namespace AprsSharp.Protocols.KISS
{
    using System;
    using System.IO.Ports;

    /// <summary>
    /// Represents an interface through a serial connection to a TNC using the KISS protocol.
    /// </summary>
    public sealed class SerialTNC : TNCInterface, IDisposable
    {
        /// <summary>
        /// The serial port to which the TNC is connected.
        /// </summary>
        private SerialPort? serialPort;

        /// <summary>
        /// Track if the object has been disposed or not.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialTNC"/> class.
        /// </summary>
        /// <param name="serialPortName">The name of the SerialPort to use.</param>
        /// <param name="port">The port on the TNC used for communication.</param>
        public SerialTNC(string? serialPortName = null, byte port = 0)
            : base(port)
        {
            if (serialPortName != null)
            {
                SetSerialPort(serialPortName);
            }
        }

        /// <summary>
        /// Sets a new serial port to use for the TNC.
        /// If an existing port is open, closes it.
        /// </summary>
        /// <param name="serialPortName">New port name.</param>
        public void SetSerialPort(string serialPortName)
        {
            if (serialPortName == null)
            {
                throw new ArgumentNullException(nameof(serialPortName));
            }

            serialPort?.Close();
            serialPort?.Dispose();

            serialPort = new SerialPort(serialPortName);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(HandleDataFromSerialPort);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            serialPort?.Dispose();
        }

        /// <inheritdoc/>
        protected override void SendToTnc(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (serialPort?.IsOpen != true)
            {
                throw new InvalidOperationException("Tried to send data when the serial port was not open.");
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
