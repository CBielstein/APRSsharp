﻿namespace AprsSharp.Connections.AprsIs
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
        /// <summary>
        /// Event raised when TCP message is returned.
        /// </summary>
        public event HandleTcpString? ReceivedTcpMessage;

        /// <summary>
        /// The method to implement the authentication and receipt of APRS packets from APRS IS server.
        /// </summary>
        /// <param name="callsign">The users callsign string.</param>
        /// <param name="password">The users password string.</param>
        /// <returns>An async task.</returns>
        public async Task Receive(string? callsign, string? password)
        {
            callsign = callsign ?? "N0CALL";
            password = password ?? "-1";
            string filter = "filter r/50.5039/4.4699/50";
            string authString = $"user {callsign} pass {password} vers AprsSharp 0.1 {filter}";
            string server = "rotate.aprs2.net";
            bool authenticated = false;

            // Open connection
            using TcpClient tcpConnection = new TcpClient();
            tcpConnection.Connect(server, 14580);

            // Set up streams
            using NetworkStream stream = tcpConnection.GetStream();
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)
            {
                NewLine = "\r\n",
                AutoFlush = true,
            };
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

           // Receive
            await Task.Run(() =>
            {
                while (true)
                {
                    string? received = reader.ReadLine();

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
                                writer.WriteLine(authString);
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
            });
        }
    }
}