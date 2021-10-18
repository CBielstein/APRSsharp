namespace AprsSharp.Shared
{
    using System;

    /// <summary>
    /// Abstracts a TCP connection.
    /// </summary>
    public interface ITcpConnection : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this connection
        /// is currently connected to a server.
        /// </summary>
        public bool Connected { get; }

        /// <summary>
        /// Opens a connection.
        /// </summary>
        /// <param name="server">Server.</param>
        /// <param name="port">Port.</param>
        void Connect(string server, int port);

        /// <summary>
        /// Receives a full line.
        /// </summary>
        /// <returns>Message as a string.</returns>
        string ReceiveString();

        /// <summary>
        /// Sends a full line.
        /// </summary>
        /// <param name="message">Message to send.</param>
        void SendString(string message);

        /// <summary>
        /// The function to stop the receipt/cancel tcp packets.
        /// </summary>
        void Disconnect();
    }
}
