namespace AprsSharp.Protocols.KISS
{
    using System;
    using System.Text;
    using AprsSharp.Shared;

    /// <summary>
    /// Represents an interface through a TCP/IP connection to a TNC using the KISS protocol.
    /// </summary>
    public sealed class TcpTNC : TNCInterface
    {
        private readonly ITcpConnection tcpConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTNC"/> class.
        /// </summary>
        /// <param name="server">Server hostname of the remote TNC.</param>
        /// <param name="port">Server TCP port of the remote TNC.</param>
        /// <param name="tncPort">Por the remote TNC should use to communicate to the radio.</param>
        public TcpTNC(string server, int port, byte tncPort)
            : base(tncPort)
        {
            tcpConnection = new TcpConnection();
            tcpConnection.Connect(server, port);
            tcpConnection.AsyncReceive(bytes => DecodeReceivedData(bytes));
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                tcpConnection.Dispose();
            }
        }

        /// <inheritdoc/>
        protected override void SendToTnc(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var message = Encoding.ASCII.GetString(bytes);
            tcpConnection.SendString(message);
        }
    }
}
