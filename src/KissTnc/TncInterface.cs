namespace AprsSharp.Protocols.KISS
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Abstracts an interface to a TNC using the KISS protocol.
    /// </summary>
    public abstract class TNCInterface
    {
        /// <summary>
        /// A queue of received bytes waiting to be delivered (at the end of a frame).
        /// </summary>
        private readonly Queue<byte> receivedBuffer = new Queue<byte>();

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
        private byte tncPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="TNCInterface"/> class.
        /// </summary>
        /// <param name="port">The port on the TNC used for communication.</param>
        public TNCInterface(byte port = 0)
        {
            SetTncPort(port);
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
        /// Sets the port on the TNC to usefor transmission.
        /// </summary>
        /// <param name="port">TNC port.</param>
        public void SetTncPort(byte port)
        {
            // ensure this is just the size of a nibble
            if (port < 0 || port > 0xF)
            {
                throw new ArgumentOutOfRangeException(nameof(port), $"Port value must be a nibble in range [0, 0xF], but was instead: {port}");
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
                throw new ArgumentNullException(nameof(bytes));
            }
            else if (bytes.Length == 0)
            {
                throw new ArgumentException("Bytes to send has length zero.", nameof(bytes));
            }

            return EncodeAndSend(Command.DataFrame, bytes);
        }

        /// <summary>
        /// Sets the transmitter keyup delay in 10 ms units.
        /// Default is 50, i.e. 500 ms.
        /// </summary>
        /// <param name="delay">Delay in 10 ms steps.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetTxDelay(byte delay)
        {
            return EncodeAndSend(Command.TxDelay, new byte[1] { delay });
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
                throw new ArgumentOutOfRangeException(nameof(p), $"p should be in range [0, 255], but was {p}");
            }

            return EncodeAndSend(Command.P, new byte[1] { p });
        }

        /// <summary>
        /// Sets slot interval in 10 ms units.
        /// Default is 10 (i.e., 100ms).
        /// </summary>
        /// <param name="slotTime">Slot interval.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetSlotTime(byte slotTime)
        {
            return EncodeAndSend(Command.SlotTime, new byte[1] { slotTime });
        }

        /// <summary>
        /// Sets time to hold TX after the frame has been sent, in 10 ms units.
        /// Obsolete, but included for backward compatibility.
        /// </summary>
        /// <param name="time">TX tail time.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetTxTail(byte time)
        {
            return EncodeAndSend(Command.TxTail, new byte[1] { time });
        }

        /// <summary>
        /// Sets duplex state.
        /// </summary>
        /// <param name="state">Half or full duplex.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetDuplexMode(DuplexState state)
        {
            byte commandByte = (state == DuplexState.FullDuplex) ? (byte)1 : (byte)0;

            return EncodeAndSend(Command.FullDuplex, new byte[1] { commandByte });
        }

        /// <summary>
        /// Sets a hardware specific value.
        /// </summary>
        /// <param name="value">The value to send to the hardware command.</param>
        /// <returns>The encoded bytes.</returns>
        public byte[] SetHardwareSpecific(byte value)
        {
            return EncodeAndSend(Command.SetHardware, new byte[1] { value });
        }

        /// <summary>
        /// Commands TNC to exit KISS mode.
        /// </summary>
        /// <returns>The encoded bytes.</returns>
        public byte[] ExitKISSMode()
        {
            return EncodeAndSend(Command.RETURN, Array.Empty<byte>());
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

            if (newBytes == null)
            {
                throw new ArgumentNullException(nameof(newBytes));
            }

            foreach (byte recByte in newBytes)
            {
                byte? byteToEnqueue = null;

                if (previousWasFEND)
                {
                    previousWasFEND = recByte == (byte)SpecialCharacter.FEND;
                    continue;
                }

                // Handle escape mode
                if (inEscapeMode)
                {
                    switch (recByte)
                    {
                        case (byte)SpecialCharacter.TFESC:
                            byteToEnqueue = (byte)SpecialCharacter.FESC;
                            break;

                        case (byte)SpecialCharacter.TFEND:
                            byteToEnqueue = (byte)SpecialCharacter.FEND;
                            break;

                        default:
                            // Not really an escape, push on the previously unused FESC character and move on
                            receivedBuffer.Enqueue((byte)SpecialCharacter.FESC);
                            break;
                    }

                    inEscapeMode = false;
                }

                // If we have already determined a byte to enqueue, no need to do this step
                if (!byteToEnqueue.HasValue)
                {
                    switch (recByte)
                    {
                        case (byte)SpecialCharacter.FEND:
                            byte[] deliverBytes = receivedBuffer.ToArray();
                            receivedBuffer.Clear();
                            previousWasFEND = true;

                            if (deliverBytes.Length > 0)
                            {
                                receivedFrames.Enqueue(deliverBytes);
                                DeliverBytes(deliverBytes);
                            }

                            break;

                        case (byte)SpecialCharacter.FESC:
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
        /// Send data to the TNC.
        /// </summary>
        /// <param name="bytes">Bytes to send to the TNC.</param>
        protected abstract void SendToTnc(byte[] bytes);

        /// <summary>
        /// Encodes a frame for KISS protocol.
        /// </summary>
        /// <param name="command">The command to use for the frame.</param>
        /// <param name="port">The port to address on the TNC.</param>
        /// <param name="bytes">Optionally, bytes to encode.</param>
        /// <returns>Encoded bytes.</returns>
        private static byte[] EncodeFrame(Command command, byte port, byte[] bytes)
        {
            // We will need at least FEND, command byte, bytes, FEND. Potentially more as bytes could have characters needing escape.
            Queue<byte> frame = new Queue<byte>(3 + ((bytes == null) ? 0 : bytes.Length));

            frame.Enqueue((byte)SpecialCharacter.FEND);
            frame.Enqueue((byte)((port << 4) | (byte)command));

            foreach (byte b in bytes ?? Array.Empty<byte>())
            {
                switch (b)
                {
                    case (byte)SpecialCharacter.FEND:
                        frame.Enqueue((byte)SpecialCharacter.FESC);
                        frame.Enqueue((byte)SpecialCharacter.TFEND);
                        break;

                    case (byte)SpecialCharacter.FESC:
                        frame.Enqueue((byte)SpecialCharacter.FESC);
                        frame.Enqueue((byte)SpecialCharacter.TFESC);
                        break;

                    default:
                        frame.Enqueue(b);
                        break;
                }
            }

            frame.Enqueue((byte)SpecialCharacter.FEND);

            return frame.ToArray();
        }

        /// <summary>
        /// Encodes and sends a frame of the given type with the given data.
        /// </summary>
        /// <param name="command">Command / frame time to encode.</param>
        /// <param name="data">Data to send.</param>
        /// <returns>The encoded bytes.</returns>
        private byte[] EncodeAndSend(Command command, byte[] data)
        {
            byte[] encodedBytes = EncodeFrame(command, tncPort, data);
            SendToTnc(encodedBytes);
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
