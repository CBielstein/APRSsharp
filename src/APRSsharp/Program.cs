namespace AprsSharp.Applications.Console
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AprsSharp.Connections.AprsIs;
    using AprsSharp.Parsers.Aprs;

    /// <summary>
    /// The public class that will be called when building the console application.
    /// It is the main class that will have functionality of calling and decoding the packets.
    /// </summary>
    public class Program
    {
        private static bool TryPrintWeatherStat(string comment, char element, string display, int length = 3)
        {
            var match = Regex.Match(comment, $"{element}(.{{{length}}})");
            if (match.Success)
            {
                Console.WriteLine($"    {display}: {match.Groups[1].Value}");
            }

            return match.Success;
        }

        /// <summary>
        /// A function matching the delegate event to print the received packet.
        /// </summary>
        /// <param name="p">A <see cref="Packet"/> to be printed.</param>
        public static void PrintPacket(Packet p)
        {
            Console.WriteLine();
            Console.WriteLine($"Received type: {p.InfoField.Type}");

            Console.WriteLine($"    Sender: {p.Sender}");
            Console.WriteLine($"    Path: {string.Join(',', p.Path)}");
            Console.WriteLine($"    Received At: {p.ReceivedTime} {p.ReceivedTime?.Kind}");
            Console.WriteLine($"    Type: {p.InfoField.Type}");

            // TODO Issue #103: Reduce copy/paste below
            // TODO Issue #103: Clean up position printing:
                // * Position lat/long encoding uses symbol IDs, not the most user-friendly
                // * Gridsquare print out should probably print the correct number of characters based on ambiguitiy
            if (p.InfoField is PositionInfo pi)
            {
                Console.WriteLine($"    Timestamp: {pi.Timestamp?.DateTime} {pi.Timestamp?.DateTime.Kind}");
                Console.WriteLine($"    Position: {pi.Position.Encode()} ({pi.Position.EncodeGridsquare(4, false)})");
                Console.WriteLine($"    Comment: {pi.Comment}");
                Console.WriteLine($"    Has Messaging: {pi.HasMessaging}");

                if (pi.Comment is not null)
                {
                    var windCombined = Regex.Match(pi.Comment, @"([0-9]{3})\/([0-9]{3})");
                    if (windCombined.Success)
                    {
                        Console.WriteLine($"    Wind direction: {windCombined.Groups[1].Value} degrees");
                        Console.WriteLine($"    Wind speed (one-minute sustained): {windCombined.Groups[2].Value} mph");
                    }
                    else
                    {
                        TryPrintWeatherStat(pi.Comment, 'c', "Wind direction (degrees)");
                        TryPrintWeatherStat(pi.Comment, 's', "Wind speed (one-minute sustained)");
                    }

                    TryPrintWeatherStat(pi.Comment, 'g', "Wind gust (5 minute max, mph)");
                    TryPrintWeatherStat(pi.Comment, 't', "Temperature (F)");
                    TryPrintWeatherStat(pi.Comment, 'r', "1-hour rainfall (100th of inch)");
                    TryPrintWeatherStat(pi.Comment, 'p', "24-hour rainfall (100th of inch)");
                    TryPrintWeatherStat(pi.Comment, 'P', "Rainfall since midnight (100th of inch)");
                    TryPrintWeatherStat(pi.Comment, 'h', "Humidity", 2);
                    TryPrintWeatherStat(pi.Comment, 'b', "Barometric pressure", 5);
                    TryPrintWeatherStat(pi.Comment, 'L', "Luminosity");
                    TryPrintWeatherStat(pi.Comment, 'l', "Luminosity (1000 + following value)");
                    TryPrintWeatherStat(pi.Comment, '#', "Raw rain");
                }
            }
            else if (p.InfoField is StatusInfo si)
            {
                Console.WriteLine($"    Timestamp: {si.Timestamp?.DateTime} {si.Timestamp?.DateTime.Kind}");
                Console.WriteLine($"    Position: {si.Position?.Encode()} ({si.Position?.EncodeGridsquare(4, false)})");
                Console.WriteLine($"    Comment: {si.Comment}");
            }
            else if (p.InfoField is MaidenheadBeaconInfo mbi)
            {
                Console.WriteLine($"    Position: {mbi.Position.EncodeGridsquare(4, false)}");
                Console.WriteLine($"    Comment: {mbi.Comment}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Main method that takes in raw packet strings.
        /// </summary>
        /// <param name="args"> The input arguments for the program i.e packets which will be strings.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            using TcpConnection tcpConnection = new TcpConnection();
            AprsIsConnection n = new AprsIsConnection(tcpConnection);
            n.ReceivedPacket += PrintPacket;

            string? input;

            // get input from the user
            Console.Write("Enter your callsign: ");
            input = Console.ReadLine();
            string callsign = !string.IsNullOrWhiteSpace(input) ? input : throw new ArgumentException("Callsign must be provided");

            Console.Write("Enter your password (optional): ");
            input = Console.ReadLine();
            string password = !string.IsNullOrWhiteSpace(input) ? input : "-1";

            Console.Write("Enter server (optional): ");
            input = Console.ReadLine();
            string server = !string.IsNullOrWhiteSpace(input) ? input : "rotate.aprs2.net";

            Console.Write("Enter your filter (optional): ");
            input = Console.ReadLine();
            string filter = !string.IsNullOrWhiteSpace(input) ? input : "r/50.5039/4.4699/50";

            await n.Receive(callsign, password, server, filter);
        }
    }
}
