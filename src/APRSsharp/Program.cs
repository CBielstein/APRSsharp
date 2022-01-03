namespace AprsSharp.Applications.Console
{
    using System;
    using System.Threading.Tasks;
    using AprsSharp.Connections.AprsIs;
    using AprsSharp.Parsers.Aprs;

    /// <summary>
    /// The public class that will be called when building the console application.
    /// It is the main class that will have functionality of calling and decoding the packets.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// A function matching the delegate event to print the received message.
        /// </summary>
        /// <param name="tcpMessage">The received tcp message that needs to be decoded and printed.</param>
        public static void PrintPacket(string tcpMessage)
        {
            Packet p;

            Console.WriteLine();
            Console.WriteLine($"Received: {tcpMessage}");

            if (tcpMessage.StartsWith('#'))
            {
                Console.WriteLine("    Server message, not decoding.");
                return;
            }

            try
            {
                p = new Packet(tcpMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    FAILED: {ex.Message}");
                return;
            }

            Console.WriteLine($"    Type: {p.InfoField.Type}");

            // TODO: Reduce copy/paste below
            // TODO: Clean up position printing:
                // * Position lat/long encoding uses symbol IDs, not the most user-friendly
                // * Gridsquare print out should probably print the correct number of characters based on ambiguitiy
            if (p.InfoField is PositionInfo pi)
            {
                Console.WriteLine($"    Timestamp: {pi.Timestamp?.DateTime} {pi.Timestamp?.DateTime.Kind}");
                Console.WriteLine($"    Position: {pi.Position.Encode()} ({pi.Position.EncodeGridsquare(4, false)})");
                Console.WriteLine($"    Comment: {pi.Comment}");
                Console.WriteLine($"    Has Messaging: {pi.HasMessaging}");
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

            // get input from the user
            Console.WriteLine("Enter your callsign: ");
            string? callsign = Console.ReadLine();
            Console.WriteLine("Enter your password: ");
            string? password = Console.ReadLine();
            n.ReceivedTcpMessage += PrintPacket;
            await n.Receive(callsign, password);
        }
    }
}
