namespace AprsSharp.Connections.AprsIs
{
    /// <summary>
    /// Tracks the state of an <see cref="AprsIsClient"/>.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// The connection has not yet been established.
        /// </summary>
        NotConnected,

        /// <summary>
        /// A connection has been made to the server.
        /// </summary>
        Connected,

        /// <summary>
        /// The server has accepted a login command.
        /// </summary>
        LoggedIn,

        /// <summary>
        /// The connection has been disconnected.
        /// </summary>
        Disconnected,
    }
}
