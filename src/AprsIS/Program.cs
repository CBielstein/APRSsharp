namespace AprsSharp.Applications.AprsIS.Connections
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// APRS console prototype.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Executes the program.
        /// </summary>
        /// <param name="args">Program args.</param>
        public static void Main(string[] args)
        {
            // Variables
            string callsign = "N0CALL"; // Standard "unidentified" callsign
            string filter = "filter r/50.503/4.4699/50"; // Radius of 50km of Belgium Space Needle 50.503887 and longitude 4.469936
            string authString = $"user {callsign} pass -1 vers AprsSharp 0.1 {filter}";
            bool authenticated = false;

            // Open connection
            using TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(@"rotate.aprs2.net", 14580);

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