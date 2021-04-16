namespace AprsSharp.Applications.Console
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
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
            // Create a root command with some options
            var rootCommand = new RootCommand
                {
                new Option<string>(
                    aliases: new string[] { "--password", "--p", "--pass" },
                    getDefaultValue: () => "-1",
                    description: "A user password whose argument is parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--callsign", "--c", "--cgn" },
                    getDefaultValue: () => "N0CALL",
                    description: "A user callsign parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--server", "--s", "--svr" },
                    getDefaultValue: () => "rotate.aprs2.net",
                    description: "A specified server parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--filter", "--f" },
                    getDefaultValue: () => "filter r/50.5039/4.4699/50",
                    description: "A user filter parsed as a string"),
                };
            rootCommand.Description = "AprsSharp console app";

            // The paremeters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string, string, string, string>(HandleAprsConnection);

            rootCommand.Invoke(args);
        }

        /// <summary>
        /// The method that will handle APRS connection and getting packets.
        /// </summary>
        /// <param name="callsign"> The user callsign that they should input.</param>
        /// <param name="password"> The user password.</param>
        /// <param name="server"> The specified server to connect.</param>
        /// <param name="filter"> The filter that will be used for receiving the packets.</param>
        public static void HandleAprsConnection(string callsign, string password, string server, string filter)
        {
            using TcpConnection tcpConnection = new TcpConnection();
            AprsIsConnection n = new AprsIsConnection(tcpConnection);
            n.ReceivedTcpMessage += PrintTcpMessage;
            n.Receive(callsign, password, server, filter);

            // skeleton method that will be used to handle the decoded packets
            Console.WriteLine("Enter the packet name ");
            var packetName = Console.ReadLine();
            Packet p = new Packet();
            p.DecodeInformationField(packetName);
            Timestamp? ts = p.Timestamp;
            Position? pos = p.Position;
            Console.WriteLine($"\nHello, your packet name is, {p.Comment}, at cordinates, {pos?.Coordinates}, and time, {ts?.DateTime.Hour}");
         }
    }
}
