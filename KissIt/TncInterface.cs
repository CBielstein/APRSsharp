using System.IO.Ports;
using System.Collections.Generic;
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
        /// The port on the TNC used for communication
        /// </summary>
        private byte tncPort = 0;

        /// <summary>
        /// Delegate function that will be called when a full frame is received and ready to be delivered
        /// </summary>
        /// <param name="sender">The TNCInterface which received the frame</param>
        /// <param name="a">The event argument, which includes the received data</param>
        public delegate void FrameReceivedEventHandler(object sender, FrameReceivedEventArgs a);

        /// <summary>
        /// The event which will be raised when a full frame is received.
        /// </summary>
        public event FrameReceivedEventHandler FrameReceivedEvent;

        /// <summary>
        /// Initializes the TNCInterface with a serial port
        /// </summary>
        /// <param name="serialPortName">The name of the SerialPort to use</param>
        /// <param name="port">The port on the TNC used for communication</param>
        public TNCInterface(string serialPortName, byte port)
        {
            serialPort = new SerialPort(serialPortName);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(TNCDataReceivedEventHandler);

            receivedBuffer = new Queue<byte>();

            tncPort = port;
        }

        /// <summary>
        /// Sends bytes on the SerialPort
        /// </summary>
        /// <param name="bytes">Bytes to send</param>
        public void SendData(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException();
            }
            else if (bytes.Length == 0)
            {
                throw new ArgumentException("Bytes to send has length zero.");
            }

            EncodeAndSend(Commands.DATA_FRAME, bytes);
        }

        /// <summary>
        /// Encodes and sends a frame of the given type with the given data
        /// </summary>
        /// <param name="command">Command / frame time to encode</param>
        /// <param name="data">Data to send</param>
        private void EncodeAndSend(Commands command, byte[] data)
        {
            byte[] encodedBytes = EncodeFrame(command, tncPort, data);
            serialPort.Write(encodedBytes, 0, encodedBytes.Length);
        }

        /// <summary>
        /// Sets the transmitter keyup delay in 10 ms units.
        /// Default is 50, i.e. 500 ms.
        /// </summary>
        /// <param name="delay">Delay in 10 ms steps</param>
        public void SetTxDelay(byte delay)
        {
            EncodeAndSend(Commands.TX_DELAY, new byte[1] { delay });
        }

        /// <summary>
        /// Sets persistence parameter p in range [0, 255].
        /// Uses formula P = p * 256 - 1.
        /// Default is p = 0.25 (such that P = 63)
        /// </summary>
        /// <param name="p">Persistence parameter in range [0, 255]</param>
        public void SetPersistenceParameter(byte p)
        {
            if (p < 0 || p > 255)
            {
                throw new ArgumentOutOfRangeException("p should be in range [0, 255], but was " + p);
            }
            EncodeAndSend(Commands.P, new byte[1] { p });
        }

        /// <summary>
        /// Sets slot interval in 10 ms units.
        /// Default is 10 (i.e., 100ms)
        /// </summary>
        /// <param name="slotTime">Slot interval</param>
        public void SetSlotTime(byte slotTime)
        {
            EncodeAndSend(Commands.SLOT_TIME, new byte[1] { slotTime });
        }

        /// <summary>
        /// Sets time to hold TX after the frame has been sent, in 10 ms units.
        /// Obsolete, but included for backward compatibility.
        /// </summary>
        /// <param name="time">TX tail time</param>
        public void SetTxTail(byte time)
        {
            EncodeAndSend(Commands.TX_TAIL, new byte[1] { time });
        }

        /// <summary>
        /// Sets duplex state.
        /// </summary>
        /// <param name="state">Half or full duplex</param>
        public void SetDuplexMode(DuplexState state)
        {
            byte commandByte = (state == DuplexState.FullDuplex) ? (byte)1 : (byte)0;

            EncodeAndSend(Commands.FULL_DUPLEX, new byte[1] { commandByte });
        }

        /// <summary>
        /// Sets a hardware specific value
        /// </summary>
        /// <param name="value">The value to send to the hardware command</param>
        public void SetHardwareSpecific(byte value)
        {
            EncodeAndSend(Commands.SET_HARDWARE, new byte[1] { value });
        }

        /// <summary>
        /// Commands TNC to exit KISS mode
        /// </summary>
        public void ExitKISSMode()
        {
            EncodeAndSend(Commands.RETURN, null);
        }

        /// <summary>
        /// Encodes a frame for KISS protocol
        /// </summary>
        /// <param name="command">The command to use for the frame</param>
        /// <param name="port">The port to address on the TNC</param>
        /// <param name="bytes">Optionally, bytes to encode</param>
        /// <returns>Encoded bytes</returns>
        private byte[] EncodeFrame(Commands command, byte port, byte[] bytes)
        {
            // We will need at least FEND, command byte, bytes, FEND. Potentially more as bytes could have characters needing escape.
            Queue<byte> frame = new Queue<byte>(3 + ((bytes == null) ? 0 : bytes.Length));

            frame.Enqueue((byte)SpecialCharacters.FEND);
            frame.Enqueue((byte)((port << 4) | (byte)command));

            if (bytes != null)
            {
                foreach (byte b in bytes)
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
            }

            frame.Enqueue((byte)SpecialCharacters.FEND);

            return frame.ToArray();
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

            FrameReceivedEvent(this, new FrameReceivedEventArgs(bytes));
        }
    }
}
