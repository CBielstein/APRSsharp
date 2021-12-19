namespace AprsSharp.Applications.Console
{
    using System;
    using System.Linq;
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
            Console.WriteLine($"Received: {tcpMessage}");

            try
            {
                Packet p = new Packet(tcpMessage);
                Console.WriteLine($"    Type: {p.InfoField.Type}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Failed to decode: {ex.Message}");
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
