namespace AprsSharp.Connections.AprsIs
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using AprsSharp.Parsers.Aprs;
    using Microsoft.Extensions.Logging;

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
    /// This class initiates connections and performs authentication
    /// to the APRS internet service for receiving packets.
    /// </summary>
    public sealed class AprsIsClient : IDisposable
    {
        private readonly ITcpConnection tcpConnection;
        private readonly TimeSpan loginPeriod = TimeSpan.FromHours(6);
        private readonly ILogger<AprsIsClient> logger;
        private bool receiving = true;
        private ConnectionState state = ConnectionState.NotConnected;
        private Timer? timer;
        private bool disposed;
        private string loginMessage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AprsIsClient"/> class.
        /// </summary>
        /// <param name="tcpConnection">An <see cref="ITcpConnection"/> to use for communication.</param>
        /// <param name="logger">An <see cref="ILogger{AprsIsClient}"/> for error/debug logging.</param>
        public AprsIsClient(ITcpConnection tcpConnection, ILogger<AprsIsClient> logger)
        {
            this.tcpConnection = tcpConnection ?? throw new ArgumentNullException(nameof(tcpConnection));
            this.logger = logger;
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
        /// Null sends no filter, which is not recommended for most clients and servers.</param>
        /// <returns>An async task.</returns>
        public async Task Receive(string callsign, string password, string server, string? filter)
        {
            try
            {
                // Open connection
                tcpConnection.Connect(server, 14580);
                State = ConnectionState.Connected;

                timer?.Dispose();
                timer = new Timer((object _) => SendLogin(callsign, password, filter), null, loginPeriod, loginPeriod);

                // Receive
                await Task.Run(() =>
                {
                    while (receiving)
                    {
                        string? received = tcpConnection.ReceiveString();
                        if (!string.IsNullOrEmpty(received))
                        {
                            ReceivedTcpMessage?.Invoke(received);

                            if (received.StartsWith('#'))
                            {
                                if (received.Contains("logresp"))
                                {
                                    State = ConnectionState.LoggedIn;
                                }

                                if (State != ConnectionState.LoggedIn)
                                {
                                    SendLogin(callsign, password, filter);
                                }
                            }
                            else if (ReceivedPacket != null)
                            {
                                try
                                {
                                    Packet p = new Packet(received);
                                    ReceivedPacket.Invoke(p);
                                }
                                catch (Exception ex)
                                {
                                    logger.LogDebug(ex, "Failed to decode packet {encodedPacked}", received);
                                }
                            }
                        }
                        else
                        {
                            Thread.Yield();
                        }
                }
            });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception encountered during receive.");
                throw;
            }
            finally
            {
                timer?.Change(Timeout.Infinite, Timeout.Infinite);
                tcpConnection.Disconnect();
                State = ConnectionState.Disconnected;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            timer?.Change(Timeout.Infinite, Timeout.Infinite);
            timer?.Dispose();
        }

        /// <summary>
        /// Sends a login message to the server.
        /// </summary>
        /// <param name="callsign">The users callsign string.</param>
        /// <param name="password">The users password string.</param>
        /// <param name="filter">The APRS-IS filter string for server-side filtering.
        /// Null sends no filter, which is not recommended for most clients and servers.</param>
        private void SendLogin(string callsign, string password, string? filter)
        {
            logger.LogInformation("Logging in to server.");

            string version = GetVersion();
            var loginMessage = $"user {callsign} pass {password} vers AprsSharp {version}";

            if (filter != null)
            {
                loginMessage += $" filter {filter}";
            }

            tcpConnection.SendString(loginMessage);
        }

        /// <summary>
        /// Uses reflection to get the assembly version as set in the csproj file.
        /// Currently, this is targeted as the version of AprsSharp.AprsIsClient package, since other projects can use this package.
        /// This works for now as all the AprsSharp versions are locked together for the time being.
        /// </summary>
        /// <returns>A semver string of the assembly version.</returns>
        private string GetVersion()
        {
            var assemblyInfo = Assembly.GetAssembly(typeof(AprsIsClient)).GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return assemblyInfo.InformationalVersion;
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
            public const string DefaultFilter = "filter r/50.5039/4.4699/50";
        }
    }
}
