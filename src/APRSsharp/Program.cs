namespace AprsSharp.Applications.Console
{
    using System;
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
        /// <param name="tcpMessage">The received tcp message that needs to be printed.</param>
        public static void PrintTcpMessage(string tcpMessage)
        {
            Console.WriteLine(tcpMessage);
        }

        /// <summary>
        /// Main method that takes in raw packet strings.
        /// </summary>
        /// <param name="args"> The input arguments for the program i.e packets which will be strings.</param>
        public static void Main(string[] args)
        {
            using TcpConnection tcpConnection = new TcpConnection();
            AprsIsConnection n = new AprsIsConnection(tcpConnection);

            // get input from the user
            Console.WriteLine("Enter your callsign: ");
            string? callsign = Console.ReadLine();
            Console.WriteLine("Enter your password: ");
            string? password = Console.ReadLine();
            n.ReceivedTcpMessage += PrintTcpMessage;
            n.Receive(callsign, password);

            // skeleton method that will be used to handle the decoded packets
            Console.WriteLine("Enter the packet name ");
            string packetName = Console.ReadLine() ?? throw new ArgumentNullException(nameof(packetName), "Did not provide packet name");
            Packet p = new Packet();
            p.DecodeInformationField(packetName);
            Timestamp? ts = p.Timestamp;
            Position? pos = p.Position;
            Console.WriteLine($"\nHello, your packet name is, {p.Comment}, at cordinates, {pos?.Coordinates}, and time, {ts?.DateTime.Hour}");
        }
    }
}
