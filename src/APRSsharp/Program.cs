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
                    aliases: new string[] { "--password","--p", "--pass" },
                    getDefaultValue: () => "-1",
                    description: "A user password whose argument is parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--callsign","--c", "--cgn" },
                    getDefaultValue: () => "N0CALL",
                    description: "A user callsign parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--server","--s", "--svr" },
                    getDefaultValue: () => "rotate.aprs2.net",
                    description: "A user server parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--filter","--f" },
                    getDefaultValue: () => "filter r/50.5039/4.4699/50",
                    description: "A user server parsed as a string")
                };
            rootCommand.Description = "APRS console app";
            Console.WriteLine("Start");

            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string, string, string, string>(HandleConnection);

            rootCommand.Invoke(args);
        }
        public static void HandleConnection(string? callsign, string? password, string? server, string? filter)
        {
            using TcpConnection tcpConnection = new TcpConnection();
            AprsIsConnection n = new AprsIsConnection(tcpConnection);
            Console.WriteLine($"The value again --password is: {password}");
            Console.WriteLine($"The value again --callsign is: {callsign}");
            //string? callsignArg = string.IsNullOrEmpty(callsign) ? null : callsign;
            //string? passwordArg = string.IsNullOrEmpty(password) ? null : password;
            n.ReceivedTcpMessage += PrintTcpMessage;
            Console.WriteLine($"The value again --password is: {password}");
            Console.WriteLine($"The value again --callsign is: {callsign}");
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
