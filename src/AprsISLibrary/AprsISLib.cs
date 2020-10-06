namespace AprsISLibrary
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Represent the library for aprs.
    /// </summary>
    public class AprsISLib
    {
        /// <summary>
        /// The connection method to the server.
        /// </summary>
        /// <param name="server">The server parameter.</param>
        public void Connect(string server)
        {
            // Not implemented.
        }

        /// <summary>
        /// The methods for receiving packets.
        /// </summary>
        public void Receive()
        {
            // Variables string callsign = "N0CALL
            string callsign = "NOCALL"; // Radius of 50km of Seattle's Space Needle
            string password = "-1";
            Console.WriteLine("empty strings input");
            Receive(callsign, password);
        }

        /// <summary>
        /// The receiving method.
        /// </summary>
        /// <param name="callsign">Specifying the different strings.</param>
        /// <param name="password">Specifying the password input strings.</param>
        public void Receive(string callsign, string password)
        {
            // Variables string callsign = "N0CALL
            string filter = "filter r/50.5039/4.4699/50"; // Radius of Belgium's Space Needle
            string authString = $"user {callsign} pass {password} vers AprsSharp 0.1 {filter}";
            string server = "rotate.aprs2.net";
            bool authenticated = false;
            Console.WriteLine("inputs");

            // Open connection
            using TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(@server, 14580);

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