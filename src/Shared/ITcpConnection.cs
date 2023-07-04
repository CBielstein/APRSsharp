namespace AprsSharp.Shared
{
    using System;

    /// <summary>
    /// Delegate for handling bytes received by this TCP connection.
    /// </summary>
    /// <param name="bytes">Bytes received.</param>
    public delegate void HandleReceivedBytes(byte[] bytes);

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
        /// Sends the contents of a byte array.
        /// </summary>
        /// <param name="bytes">Bytes to be sent.</param>
        void SendBytes(byte[] bytes);

        /// <summary>
        /// The function to stop the receipt/cancel tcp packets.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Begins async IO receive and accepts callback to handle new bytes.
        /// </summary>
        /// <param name="callback">Delegate to handle received bytes.</param>
        void AsyncReceive(HandleReceivedBytes callback);
    }
}
