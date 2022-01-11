namespace AprsSharp.Connections.AprsIs
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate for handling a full string from a TCP client.
    /// </summary>
    /// <param name="tcpMessage">The TCP message.</param>
    public delegate void HandleTcpString(string tcpMessage);

    /// <summary>
    /// This class initiates connections and performs authentication to the APRS internet service for receiving packets.
    /// It gives a user an option to use default credentials, filter and server or login with their specified user information.
    /// </summary>
    public class AprsIsConnection
    {
        private readonly ITcpConnection tcpConnection;

        /// <summary>
        /// Static class that defines different constants.
        /// <summary>
        public static class AprsIsConstants
        {
            public const string DefaultCallsign = "N0CALL";
            public const string DefaultPassword = "-1";
            public const string DefaultServerName = "rotate.aprs2.net";
            public const string DefaultFilter = "filter r/50.5039/4.4699/50";
        }

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
        /// The method to implement the authentication and receipt of APRS packets from APRS IS server.
        /// </summary>
        /// <param name="callsign">The users callsign string.</param>
        /// <param name="password">The users password string.</param>
        /// <param name="server">The specified server to be connected.</param>
        /// <param name="filter">The filter that is put for the connection.</param>
        /// <returns>An async task.</returns>
        public async Task Receive(string? callsign, string? password, string? server, string? filter)
        {
            string authString = $"user {callsign} pass {password} vers AprsSharp 0.1 {filter}";
            bool authenticated = false;

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
                                authenticated = true;
                            }

                            if (!authenticated)
                            {
                                tcpConnection.SendString(authString);
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
            });
        }
    }
}