namespace AprsSharp.Connections.AprsIs
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// This class initiates connections and performs authentication to the APRS internet service for receiving packets.
    /// It gives a user an option to use default credentials, filter and server or login with their specified user information.
    /// </summary>
    public class AprsIsConnection
    {
        /// <summary>
        /// The method to implement the authentication and receipt of APRS packets from APRS IS server.
        /// </summary>
        /// <param name="callsign">The users callsign string.</param>
        /// <param name="password">The users password string.</param>
        public void Receive(string? callsign, string? password)
        {
            callsign = callsign ?? "N0CALL";
            password = password ?? "-1";
            string filter = "filter r/50.5039/4.4699/50";
            string authString = $"user {callsign} pass {password} vers AprsSharp 0.1 {filter}";
            string server = "rotate.aprs2.net";
            bool authenticated = false;

            // Open connection
            using TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(server, 14580);

            // Set up streams
            using NetworkStream stream = tcpClient.GetStream();
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)
            {
                NewLine = "\r\n",
                AutoFlush = true,
            };
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

            // Receive
            while (true)
            {
                Thread.Sleep(500);

                string? received = reader.ReadLine();

                if (!string.IsNullOrEmpty(received))
                {
                    Console.WriteLine(received);

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
            }
        }
    }
}