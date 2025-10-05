namespace AprsSharp.KissTnc
{
    using System;
    using AprsSharp.Shared;

    /// <summary>
    /// Represents an interface through a TCP/IP connection to a TNC using the KISS protocol.
    /// </summary>
    public sealed class TcpTnc : Tnc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP008:Don't assign member with injected and created disposables",
            Justification = "Used for testing, disposal managed by the tcpConnectionInjected bool.")]
        private readonly ITcpConnection tcpConnection;
        private readonly bool tcpConnectionInjected;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTnc"/> class.
        /// </summary>
        /// <param name="tcpConnection">A <see cref="ITcpConnection"/> to communicate with the TNC.</param>
        /// <param name="tncPort">Port the remote TNC should use to communicate to the radio. Part of the KISS protocol.</param>
        public TcpTnc(ITcpConnection tcpConnection, byte tncPort)
            : base(tncPort)
        {
            tcpConnectionInjected = true;
            this.tcpConnection = tcpConnection ?? throw new ArgumentNullException(nameof(tcpConnection));

            if (!this.tcpConnection.Connected)
            {
                throw new ArgumentException("TCP connection is not yet connected.", nameof(tcpConnection));
            }

            this.tcpConnection.AsyncReceive(bytes => DecodeReceivedData(bytes));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTnc"/> class.
        /// </summary>
        /// <param name="address">The address (e.g. IP or domain name) of the TCP server.</param>
        /// <param name="tcpPort">The TCP port for connection to the TCP server.</param>
        /// <param name="tncPort">The TNC port used for connection to the radio. Part of the KISS protocol.</param>
        public TcpTnc(string address, int tcpPort, byte tncPort)
            : base(tncPort)
        {
            tcpConnectionInjected = false;
            tcpConnection = new TcpConnection();
            tcpConnection.Connect(address, tcpPort);

            tcpConnection.AsyncReceive(bytes => DecodeReceivedData(bytes));
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (tcpConnectionInjected == false)
                {
                    tcpConnection.Dispose();
                }
            }

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
