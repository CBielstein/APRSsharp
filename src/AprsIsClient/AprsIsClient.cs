namespace AprsSharp.AprsIsClient
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AprsSharp.AprsParser;
    using AprsSharp.Shared;

    /// <summary>
    /// Delegate for handling a full string from a TCP client.
    /// </summary>
    /// <param name="tcpMessage">The TCP message.</param>
    public delegate void HandleTcpString(string tcpMessage);

    /// <summary>
    /// Delegate for handling a decoded APRS packet.
    /// </summary>
    /// <param name="packet">Decoded APRS <see cref="Packet"/>.</param>
    public delegate void HandlePacket(Packet packet);

    /// <summary>
    /// Delegate for handling a state change in an <see cref="AprsIsClient"/>.
    /// </summary>
    /// <param name="state">The new state taken by the <see cref="AprsIsClient"/>.</param>
    public delegate void HandleStateChange(ConnectionState state);

    /// <summary>
    /// Delegate for handling a failed packet decode.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> that occurred.</param>
    /// <param name="encodedPacket">The string that failed to decode..</param>
    public delegate void HandleParseExcpetion(Exception ex, string encodedPacket);

    /// <summary>
    /// This class initiates connections and performs authentication
    /// to the APRS internet service for receiving packets.
    /// </summary>
    public sealed class AprsIsClient : IDisposable
    {
        private static string loginResponseRegex = @"^# logresp (.+) (unverified|verified), server (\S+)";
        private readonly ITcpConnection tcpConnection;
        private readonly bool disposeITcpConnection;
        private readonly TimeSpan loginPeriod = TimeSpan.FromHours(6);
        private bool receiving = true;
        private ConnectionState state = ConnectionState.NotConnected;
        private Timer? keepAliveTimer;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AprsIsClient"/> class.
        /// </summary>
        /// <param name="tcpConnection">An <see cref="ITcpConnection"/> to use for communication.</param>
        /// <param name="disposeConnection">`true` if the <see cref="ITcpConnection"/> should be disposed by <see cref="Dispose"/>,
        ///     `false` if you intend to reuse the <see cref="ITcpConnection"/>.</param>
        public AprsIsClient(ITcpConnection tcpConnection, bool disposeConnection = true)
        {
            this.tcpConnection = tcpConnection ?? throw new ArgumentNullException(nameof(tcpConnection));
            disposeITcpConnection = disposeConnection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AprsIsClient"/> class.
        /// </summary>
        public AprsIsClient()
            : this(new TcpConnection(), true)
        {
        }

        /// <summary>
        /// Event raised when TCP message is returned.
        /// </summary>
        public event HandleTcpString? ReceivedTcpMessage;

        /// <summary>
        /// Event raised when an APRS packet is received and decoded.
        /// </summary>
        public event HandlePacket? ReceivedPacket;

        /// <summary>
        /// Event raised when <see cref="State"/> changes.
        /// </summary>
        public event HandleStateChange? ChangedState;

        /// <summary>
        /// Event raised when an error or exception is encountered during packet parsing.
        /// </summary>
        public event HandleParseExcpetion? DecodeFailed;

        /// <summary>
        /// Gets the state of this connection.
        /// </summary>
        public ConnectionState State
        {
            get => state;
            private set
            {
                state = value;
                ChangedState?.Invoke(value);
            }
        }

        /// <summary>
        /// Gets the name of the currently logged in server.
        /// </summary>
        public string? ConnectedServer { get; private set; } = null;

        /// <summary>
        /// Method to cancel the receipt of packets.
        /// </summary>
        public void Disconnect()
        {
            receiving = false;
        }

        /// <summary>
        /// The method to implement the authentication and receipt of APRS packets from APRS IS server.
        /// </summary>
        /// <param name="callsign">The users callsign string.</param>
        /// <param name="password">The users password string.</param>
        /// <param name="server">The APRS-IS server to contact.</param>
        /// <param name="filter">The APRS-IS filter string for server-side filtering.
        /// Null sends no filter, which is not recommended for most clients and servers.
        /// This parameter shouldn't include the `filter` at the start, just the logic string itself.</param>
        /// <returns>An async task.</returns>
        public async Task Receive(string callsign, string password, string server, string? filter)
        {
            try
            {
                // Open connection
                tcpConnection.Connect(server, 14580);
                State = ConnectionState.Connected;

                keepAliveTimer?.Dispose();
                keepAliveTimer = new Timer((object _) => SendLogin(callsign, password, filter), null, loginPeriod, loginPeriod);

                // Receive
                await Task.Run(() =>
                {
                    while (receiving && tcpConnection.Connected)
                    {
                        string? received = tcpConnection.ReceiveString();
                        if (!string.IsNullOrEmpty(received))
                        {
                            ReceivedTcpMessage?.Invoke(received);

                            if (received.StartsWith('#'))
                            {
                                if (received.StartsWith("# logresp"))
                                {
                                    ConnectedServer = GetConnectedServer(received);
                                    State = ConnectionState.LoggedIn;
                                }

                                if (State != ConnectionState.LoggedIn)
                                {
                                    SendLogin(callsign, password, filter);
                                }
                            }
                            else if (ReceivedPacket != null)
                            {
                                // Only attempt to decode if the user
                                // wants to receive packets instead of
                                // only TCP messages
                                try
                                {
                                    Packet p = new Packet(received);
                                    ReceivedPacket.Invoke(p);
                                }
                                catch (Exception ex)
                                {
                                    DecodeFailed?.Invoke(ex, received);
                                }
                            }
                        }
                    }
                });
            }
            finally
            {
                keepAliveTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                tcpConnection.Disconnect();
                State = ConnectionState.Disconnected;
            }
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP007", Justification = "Guarding with boolean flag passed to constructor.")]
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            Disconnect();

            if (disposeITcpConnection)
            {
                tcpConnection.Dispose();
            }

            keepAliveTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            keepAliveTimer?.Dispose();
        }

        /// <summary>
        /// Gets the connected server from a login response. Null if server name cannot be parsed.
        /// </summary>
        /// <param name="loginResponse">The login response string from the APRS server.</param>
        private static string GetConnectedServer(string loginResponse)
        {
            Match match = Regex.Match(loginResponse, loginResponseRegex);
            if (match.Success)
            {
                return match.Groups[3].Value;
            }
            else
            {
                throw new ArgumentException($"Could not parse server login response message: {loginResponse}");
            }
        }

        /// <summary>
        /// Sends a login message to the server.
        /// </summary>
        /// <param name="callsign">The users callsign string.</param>
        /// <param name="password">The users password string.</param>
        /// <param name="filter">The APRS-IS filter string for server-side filtering.
        /// Null sends no filter, which is not recommended for most clients and servers.
        /// This parameter shouldn't include the `filter` at the start, just the logic string itself.</param>
        private void SendLogin(string callsign, string password, string? filter)
        {
            string version = Utilities.GetAssemblyVersion();
            var loginMessage = $"user {callsign} pass {password} vers AprsSharp {version}";

            if (filter != null)
            {
                loginMessage += $" filter {filter}";
            }

            tcpConnection.SendString(loginMessage);
        }

        /// <summary>
        /// Static class that defines different constants.
        /// </summary>
        public static class AprsIsConstants
        {
            /// <summary>
            /// This defines the default callsign.
            /// </summary>
            public const string DefaultCallsign = "N0CALL";

            /// <summary>
            /// This defines the default password.
            /// </summary>
            public const string DefaultPassword = "-1";

            /// <summary>
            /// This defines the default server to connect to.
            /// </summary>
            public const string DefaultServerName = "rotate.aprs2.net";

            /// <summary>
            /// This defines the default filter.
            /// </summary>
            public const string DefaultFilter = "r/50.5039/4.4699/50";
        }
    }
}
