namespace AprsSharp.Protocols.KISS
{
    using System;
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
        /// <param name="tcpConnection">A <see cref="ITcpConnection"/> to communicate with the TNC.</param>
        /// <param name="tncPort">Por the remote TNC should use to communicate to the radio.</param>
        public TcpTNC(ITcpConnection tcpConnection, byte tncPort)
            : base(tncPort)
        {
            this.tcpConnection = tcpConnection ?? throw new ArgumentNullException(nameof(tcpConnection));

            if (!this.tcpConnection.Connected)
            {
                throw new ArgumentException("TCP connection is not yet connected.", nameof(tcpConnection));
            }

            this.tcpConnection.AsyncReceive(bytes => DecodeReceivedData(bytes));
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

            tcpConnection.SendBytes(bytes);
        }
    }
}
