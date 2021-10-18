namespace AprsSharp.Protocols.KISS
{
    using System;
    using System.Text;
    using AprsSharp.Shared;

    /// <summary>
    /// Represents an interface through a TCP/IP connection to a TNC using the KISS protocol.
    /// TODO: Standarize casing of names.
    /// </summary>
    public sealed class TcpTnc : TNCInterface, IDisposable
    {
        private readonly ITcpConnection tcpConnection;

        /// <summary>
        /// Track if the object has been disposed or not.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTnc"/> class.
        /// </summary>
        /// <param name="server">Server hostname of the remote TNC.</param>
        /// <param name="port">Server TCP port of the remote TNC.</param>
        /// <param name="tncPort">Por the remote TNC should use to communicate to the radio.</param>
        public TcpTnc(string server, int port, byte tncPort)
            : base(tncPort)
        {
            tcpConnection = new TcpConnection();
            tcpConnection.Connect(server, port);
            tcpConnection.AsyncReceive(bytes => DecodeReceivedData(bytes));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            tcpConnection.Dispose();
        }

        /// <inheritdoc/>
        protected override void SendToTnc(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var message = Encoding.UTF8.GetString(bytes);
            tcpConnection.SendString(message);
        }
    }
}
