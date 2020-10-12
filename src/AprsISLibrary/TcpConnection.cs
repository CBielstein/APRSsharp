namespace AprsISLibrary
{
    using System;
    using System.IO;
    using System.Net.Sockets;

    /// <summary>
    /// Represents a TcpConnection.
    /// </summary>
    public class TcpConnection : ITcpConnection, IDisposable
    {
        private readonly TcpClient tcpClient = new TcpClient();
        private NetworkStream? stream;
        private StreamWriter? writer;
        private StreamReader? reader;

        /// <inheritdoc/>
        public void Connect(string server, int port)
        {
            tcpClient.Connect(@server, 14580);
            stream = tcpClient.GetStream();
            writer = new StreamWriter(stream)
            {
                NewLine = "\r\n",
                AutoFlush = true,
            };
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
            if (writer == null)
            {
                throw new Exception();
            }

            writer.WriteLine(message);
        }
    }
}
