namespace AprsSharp.Shared
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// Represents a TcpConnection.
    /// </summary>
    public sealed class TcpConnection : ITcpConnection
    {
        private readonly TcpClient tcpClient = new TcpClient();
        private NetworkStream? stream;
        private StreamWriter? writer;
        private StreamReader? reader;

        /// <inheritdoc/>
        public bool Connected => tcpClient.Connected;

        /// <inheritdoc/>
        public void Connect(string server, int port)
        {
            tcpClient.Connect(server, port);
            stream?.Dispose();
            stream = tcpClient.GetStream();
            writer?.Dispose();
            writer = new StreamWriter(stream)
            {
                NewLine = "\r\n",
                AutoFlush = true,
            };
            reader?.Dispose();
            reader = new StreamReader(stream);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            writer?.Dispose();
            reader?.Dispose();
            stream?.Dispose();
            tcpClient?.Dispose();
        }

        /// <inheritdoc/>
        public string ReceiveString()
        {
            if (reader == null)
            {
                throw new Exception();
            }

            return reader.ReadLine();
        }

        /// <inheritdoc/>
        public void SendString(string message)
        {
            var bytes = Encoding.ASCII.GetBytes(message);
            SendBytes(bytes);
        }

        /// <inheritdoc/>
        public void SendBytes(byte[] bytes)
        {
            if (writer == null)
            {
                throw new Exception();
            }

            writer.BaseStream.Write(bytes);
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            stream?.Close();
            tcpClient.Close();
        }

        /// <inheritdoc/>
        public void AsyncReceive(HandleReceivedBytes callback)
        {
            var buffer = new byte[10];

            if (stream?.CanRead != true)
            {
                throw new InvalidOperationException("Cannot read from NetworkStream.");
            }

            var receiveState = new ReceiveState(stream, buffer, callback);

            stream.BeginRead(buffer, 0, buffer.Length, HandleReceive, receiveState);
        }

        /// <summary>
        /// Handles receive of new bytes by invoking the callback and continuing receive.
        /// </summary>
        /// <param name="asyncResult">AsyncResult carrying the ReceiveState of this async IO.</param>
        private static void HandleReceive(IAsyncResult asyncResult)
        {
            var rs = (ReceiveState)asyncResult.AsyncState;

            var bytesRead = rs.Stream.EndRead(asyncResult);
            var receivedBytes = rs.Buffer.AsSpan(0, bytesRead).ToArray();

            rs.Callback(receivedBytes);

            rs.Stream.BeginRead(rs.Buffer, 0, rs.Buffer.Length, HandleReceive, rs);
        }

        /// <summary>
        /// Private class to track the state of async IO.
        /// </summary>
        private class ReceiveState
        {
            public ReceiveState(NetworkStream stream, byte[] buffer, HandleReceivedBytes callback)
            {
                Stream = stream;
                Buffer = buffer;
                Callback = callback;
            }

            /// <summary>
            /// Gets or sets the NetworkStream being used for receive.
            /// </summary>
            public NetworkStream Stream { get; set; }

            /// <summary>
            /// Gets or sets the buffer being used to store recieved bytes.
            /// </summary>
            public byte[] Buffer { get; set; }

            /// <summary>
            /// Gets or sets the callback to invoke when bytes are received.
            /// </summary>
            public HandleReceivedBytes Callback { get; set; }
        }
    }
}
