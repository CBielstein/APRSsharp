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
    /// Represent the library for aprs.
    /// </summary>
    public class AprsIsConnection
    {
        private readonly ITcpConnection tcpConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AprsIsConnection"/> class.
        /// </summary>
        /// <param name="tcpConnection">Optionally, a TcpConnection to use for communication.</param>
        public AprsIsConnection(ITcpConnection? tcpConnection = null)
        {
            this.tcpConnection = tcpConnection ?? new TcpConnection();
        }

        /// <summary>
        /// Event raised when TCP message is returned.
        /// </summary>
        public event HandleTcpString? ReceivedTcpMessage;

        /// <summary>
        /// The receiving method.
        /// </summary>
        /// <param name="callsign">Specifying the different strings.</param>
        /// <param name="password">Specifying the password input strings.</param>
        /// <returns>An async task.</returns>
        public async Task Receive(string? callsign, string? password)
        {
            // Variables string callsign = "N0CALL
            callsign = callsign ?? "N0CALL";
            password = password ?? "-1";
            string filter = "filter r/50.5039/4.4699/50"; // Radius of Belgium's Space Needle
            string authString = $"user {callsign} pass {password} vers AprsSharp 0.1 {filter}";
            string server = "rotate.aprs2.net";
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
                        Console.WriteLine(received);
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