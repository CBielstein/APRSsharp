namespace AprsSharp.Connections.AprsIs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AprsSharp.Parsers.Aprs;

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
    /// This class initiates connections and performs authentication to the APRS internet service for receiving packets.
    /// It gives a user an option to use default credentials, filter and server or login with their specified user information.
    /// </summary>
    public class AprsIsConnection
    {
        private readonly ITcpConnection tcpConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AprsIsConnection"/> class.
        /// </summary>
        /// <param name="tcpConnection">An <see cref="ITcpConnection"/> to use for communication.</param>
        public AprsIsConnection(ITcpConnection tcpConnection)
        {
            if (tcpConnection == null)
            {
                throw new ArgumentNullException(nameof(tcpConnection));
            }

            this.tcpConnection = tcpConnection;
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
        /// Gets a value indicating whether this connection is logged in to the server.
        /// Note that this is not the same as successful password authentication.
        /// </summary>
        public bool LoggedIn { get; private set; } = false;

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
            string loginMessage = $"user {callsign} pass {password} vers AprsSharp 0.1";
            if (filter != null)
            {
                loginMessage += $" filter {filter}";
            }

            // Open connection
            tcpConnection.Connect(server, 14580);

            // Receive
            await Task.Run(() =>
            {
                while (true)
                {
                    string? received = tcpConnection.ReceiveString();
                    if (!string.IsNullOrEmpty(received))
                    {
                        ReceivedTcpMessage?.Invoke(received);

                        if (received.StartsWith('#'))
                        {
                            if (received.Contains("logresp"))
                            {
                                LoggedIn = true;
                            }

                            if (!LoggedIn)
                            {
                                tcpConnection.SendString(loginMessage);
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
                                Console.Error.WriteLine($"Failed to decode packet {received} with error {ex}");
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
