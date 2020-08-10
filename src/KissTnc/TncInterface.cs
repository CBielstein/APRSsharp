namespace AprsSharp.Protocols
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;

    /// <summary>
    /// Represents an interface through a serial connection to a TNC using the KISS protocol.
    /// </summary>
    public class TNCInterface
    {
        /// <summary>
        /// The serial port to which the TNC is connected.
        /// </summary>
        private SerialPort? serialPort;

        /// <summary>
        /// A queue of received bytes waiting to be delivered (at the end of a frame).
        /// </summary>
        private Queue<byte> receivedBuffer;

        /// <summary>
        /// Marks if the next character received should be translated as an escaped character.
        /// </summary>
        private bool inEscapeMode = false;

        /// <summary>
        /// Tracks if the previously received byte was FEND, so we can skip the control byte which comes next.
        /// </summary>
        private bool previousWasFEND = false;

        /// <summary>
        /// The port on the TNC used for communication.
        /// </summary>
        private byte tncPort = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TNCInterface"/> class.
        /// </summary>
        /// <param name="serialPortName">The name of the SerialPort to use.</param>
        /// <param name="port">The port on the TNC used for communication.</param>
        public TNCInterface(string? serialPortName, byte port)
        {
            if (serialPortName != null)
            {
                SetSerialPort(serialPortName);
            }

            receivedBuffer = new Queue<byte>();

            SetTncPort(port);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TNCInterface"/> class.
        /// </summary>
        public TNCInterface()
            : this(null, 0)
        {
        }

        /// <summary>
        /// Delegate function that will be called when a full frame is received and ready to be delivered.
        /// </summary>
        /// <param name="sender">The TNCInterface which received the frame.</param>
        /// <param name="a">The event argument, which includes the received data.</param>
        public delegate void FrameReceivedEventHandler(object sender, FrameReceivedEventArgs a);

        /// <summary>
        /// The event which will be raised when a full frame is received.
        /// </summary>
        public event FrameReceivedEventHandler? FrameReceivedEvent;

        /// <summary>
        /// Sets a new serial port to use for the TNC.
        /// If an existing port is open, closes it.
        /// </summary>
        /// <param name="serialPortName">New port name.</param>
        public void SetSerialPort(string serialPortName)
        {
            if (serialPortName == null)
            {
                throw new ArgumentNullException("serialPortName");
            }

            serialPort?.Close();

            serialPort = new SerialPort(serialPortName);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(TNCDataReceivedEventHandler);
        }

        /// <summary>
        /// Sets the port on the TNC to usefor transmission.
        /// </summary>
        /// <param name="port">TNC port.</param>
        public void SetTncPort(byte port)
        {
            // ensure this is just the size of a nibble
            if (port < 0 || port > 0xF)
            {
                throw new ArgumentOutOfRangeException("port", "Port value must be a nibble in range [0, 0xF], but was instead " + port);
            }

            tncPort = port;
        }

        /// <summary>
        /// Sends bytes on the SerialPort.
        /// </summary>
        /// <param name="bytes">Bytes to send.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SendData(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            else if (bytes.Length == 0)
            {
                throw new ArgumentException("Bytes to send has length zero.", "bytes");
            }

            return EncodeAndSend(Commands.DATA_FRAME, bytes);
        }

        /// <summary>
        /// Sets the transmitter keyup delay in 10 ms units.
        /// Default is 50, i.e. 500 ms.
        /// </summary>
        /// <param name="delay">Delay in 10 ms steps.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetTxDelay(byte delay)
        {
            return EncodeAndSend(Commands.TX_DELAY, new byte[1] { delay });
        }

        /// <summary>
        /// Sets persistence parameter p in range [0, 255].
        /// Uses formula P = p * 256 - 1.
        /// Default is p = 0.25 (such that P = 63).
        /// </summary>
        /// <param name="p">Persistence parameter in range [0, 255].</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetPersistenceParameter(byte p)
        {
            if (p < 0 || p > 255)
            {
                throw new ArgumentOutOfRangeException("p", "p should be in range [0, 255], but was " + p);
            }

            return EncodeAndSend(Commands.P, new byte[1] { p });
        }

        /// <summary>
        /// Sets slot interval in 10 ms units.
        /// Default is 10 (i.e., 100ms).
        /// </summary>
        /// <param name="slotTime">Slot interval.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetSlotTime(byte slotTime)
        {
            return EncodeAndSend(Commands.SLOT_TIME, new byte[1] { slotTime });
        }

        /// <summary>
        /// Sets time to hold TX after the frame has been sent, in 10 ms units.
        /// Obsolete, but included for backward compatibility.
        /// </summary>
        /// <param name="time">TX tail time.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetTxTail(byte time)
        {
            return EncodeAndSend(Commands.TX_TAIL, new byte[1] { time });
        }

        /// <summary>
        /// Sets duplex state.
        /// </summary>
        /// <param name="state">Half or full duplex.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetDuplexMode(DuplexState state)
        {
            byte commandByte = (state == DuplexState.FullDuplex) ? (byte)1 : (byte)0;

            return EncodeAndSend(Commands.FULL_DUPLEX, new byte[1] { commandByte });
        }

        /// <summary>
        /// Sets a hardware specific value.
        /// </summary>
        /// <param name="value">The value to send to the hardware command.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetHardwareSpecific(byte value)
        {
            return EncodeAndSend(Commands.SET_HARDWARE, new byte[1] { value });
        }

        /// <summary>
        /// Commands TNC to exit KISS mode.
        /// </summary>
        /// <returns>The encoded bytes.</returns>
        public byte[] ExitKISSMode()
        {
            return EncodeAndSend(Commands.RETURN, Array.Empty<byte>());
        }

        /// <summary>
        /// Adds bytes to the queue, dequeues a complete frame if available.
        /// This is public and returns the decoded frames to allow for handling
        /// the connection to the TNC separately. However, if you're using a serial
        /// TNC connection, it's likely easiest to set handlers and not call this function directly.
        /// </summary>
        /// <param name="newBytes">Bytes which have just arrived.</param>
        /// <returns>Array of decoded frames as byte arrays.</returns>
        public byte[][] DecodeReceivedData(byte[] newBytes)
        {
            Queue<byte[]> receivedFrames = new Queue<byte[]>();

            foreach (byte recByte in newBytes)
            {
                byte? byteToEnqueue = null;

                if (previousWasFEND)
                {
                    previousWasFEND = recByte == (byte)SpecialCharacters.FEND;
                    continue;
                }

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
                            previousWasFEND = true;

                            if (deliverBytes.Length > 0)
                            {
                                receivedFrames.Enqueue(deliverBytes);
                                DeliverBytes(deliverBytes);
                            }

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

            return receivedFrames.ToArray();
        }

        /// <summary>
        /// Encodes a frame for KISS protocol.
        /// </summary>
        /// <param name="command">The command to use for the frame.</param>
        /// <param name="port">The port to address on the TNC.</param>
        /// <param name="bytes">Optionally, bytes to encode.</param>
        /// <returns>Encoded bytes.</returns>
        private byte[] EncodeFrame(Commands command, byte port, byte[] bytes)
        {
            // We will need at least FEND, command byte, bytes, FEND. Potentially more as bytes could have characters needing escape.
            Queue<byte> frame = new Queue<byte>(3 + ((bytes == null) ? 0 : bytes.Length));

            frame.Enqueue((byte)SpecialCharacters.FEND);
            frame.Enqueue((byte)((port << 4) | (byte)command));

            foreach (byte b in bytes ?? Array.Empty<byte>())
            {
                switch (b)
                {
                    case (byte)SpecialCharacters.FEND:
                        frame.Enqueue((byte)SpecialCharacters.FESC);
                        frame.Enqueue((byte)SpecialCharacters.TFEND);
                        break;

                    case (byte)SpecialCharacters.FESC:
                        frame.Enqueue((byte)SpecialCharacters.FESC);
                        frame.Enqueue((byte)SpecialCharacters.TFESC);
                        break;

                    default:
                        frame.Enqueue(b);
                        break;
                }
            }

            frame.Enqueue((byte)SpecialCharacters.FEND);

            return frame.ToArray();
        }

        /// <summary>
        /// Handle the DataReceived event from the serialPort.
        /// </summary>
        /// <param name="sender">SerialPort which has received data.</param>
        /// <param name="args">Additional args.</param>
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

            DecodeReceivedData(bytesReceived);
        }

        /// <summary>
        /// Encodes and sends a frame of the given type with the given data.
        /// </summary>
        /// <param name="command">Command / frame time to encode.</param>
        /// <param name="data">Data to send.</param>
        /// <returns>The encoded bytes.</returns>
        private byte[] EncodeAndSend(Commands command, byte[] data)
        {
            byte[] encodedBytes = EncodeFrame(command, tncPort, data);

            if (serialPort?.IsOpen == true)
            {
                serialPort.Write(encodedBytes, 0, encodedBytes.Length);
            }

            return encodedBytes;
        }

        /// <summary>
        /// Handles delivery of bytes. Called when the contents of the buffer are ready to be delivered.
        /// </summary>
        /// <param name="bytes">Bytes to be delivered.</param>
        private void DeliverBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            FrameReceivedEvent?.Invoke(this, new FrameReceivedEventArgs(bytes));
        }
    }
}
