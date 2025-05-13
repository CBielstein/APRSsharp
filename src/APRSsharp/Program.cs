namespace AprsSharp.Applications.Console
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AprsSharp.AprsIsClient;
    using AprsSharp.AprsParser;
    using AprsSharp.KissTnc;
    using AprsSharp.Shared;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The public class that will be called when building the console application.
    /// It is the main class that will have functionality of calling and decoding the packets.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// If true, display unsupported <see cref="InfoField"/> types as raw encoding.
        /// </summary>
        private static bool displayUnsupported = false;

        /// <summary>
        /// A function matching the delegate event to print the received packet.
        /// </summary>
        /// <param name="p">A <see cref="Packet"/> to be printed.</param>
        public static void PrintPacket(Packet p)
        {
            ArgumentNullException.ThrowIfNull(p);
            if (!Program.displayUnsupported && p.InfoField is UnsupportedInfo)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"Received type: {p.InfoField.Type}");

            Console.WriteLine($"    Sender: {p.Sender}");
            Console.WriteLine($"    Destination: {p.Destination}");
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

                if (p.InfoField is WeatherInfo wi)
                {
                    Console.WriteLine($"    Wind direction (degrees): {wi.WindDirection}");
                    Console.WriteLine($"    Wind speed (one-minute sustained): {wi.WindSpeed}");
                    Console.WriteLine($"    Wind gust (5 minute max, mph): {wi.WindGust}");
                    Console.WriteLine($"    Temperature (F): {wi.Temperature}");
                    Console.WriteLine($"    1-hour rainfall (100th of inch): {wi.Rainfall1Hour}");
                    Console.WriteLine($"    24-hour rainfall (100th of inch): {wi.Rainfall24Hour}");
                    Console.WriteLine($"    Rainfall since midnight (100th of inch): {wi.RainfallSinceMidnight}");
                    Console.WriteLine($"    Humidity: {wi.Humidity}");
                    Console.WriteLine($"    Barometric pressure: {wi.BarometricPressure}");
                    Console.WriteLine($"    Luminosity: {wi.Luminosity}");
                    Console.WriteLine($"    Raw rain: {wi.RainRaw}");
                    Console.WriteLine($"    Snow (inches, last 24 hours): {wi.Snow}");
                    Console.WriteLine($"    Weather Comment: {wi.Comment}");
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
            else if (p.InfoField is MessageInfo mi)
            {
                Console.WriteLine($"    To: {mi.Addressee}");
                Console.WriteLine($"    Message: {mi.Content}");
                Console.WriteLine($"    ID: {mi.Id}");
            }
            else if (p.InfoField is UnsupportedInfo ui)
            {
                Console.WriteLine($"    Content: {ui.Content}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Main method that takes in raw packet strings.
        /// </summary>
        /// <param name="args"> Parameters to be passed as commandline arguments.</param>
        public static void Main(string[] args)
        {
            // Create a root command with some options
#pragma warning disable CA1861 // Avoid constant arrays as arguments
            var rootCommand = new RootCommand
                {
                new Option<Mode>(
                    aliases: new string[] { "--mode", "-m" },
                    getDefaultValue: () => Mode.APRSIS,
                    description: "The mode of operation for this program"),
                new Option<string>(
                    aliases: new string[] { "--callsign", "-c", "--cgn" },
                    getDefaultValue: () => AprsIsClient.AprsIsConstants.DefaultCallsign,
                    description: "A user callsign parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--password", "-p", "--pwd", "--pass" },
                    getDefaultValue: () => AprsIsClient.AprsIsConstants.DefaultPassword,
                    description: "A user password whose argument is parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--server", "-s", "--svr" },
                    getDefaultValue: () => AprsIsClient.AprsIsConstants.DefaultServerName,
                    description: "A specified server parsed as a string, used for APRS-IS or TCP TNCs"),
                new Option<int>(
                    aliases: new string[] { "--port" },
                    getDefaultValue: () => 8001,
                    description: "A TCP port for use with TCP TNCs"),
                new Option<string>(
                    aliases: new string[] { "--filter", "-f" },
                    getDefaultValue: () => AprsIsClient.AprsIsConstants.DefaultFilter,
                    description: "A user filter parsed as a string. Should not include the word 'filter', just the logic string."),
                new Option(
                    aliases: new string[] { "--display-unsupported" },
                    description: "If specified, includes output of unknown or unsupported info types values. If not, such packets are not displayed."),
                new Option<string?>(
                    aliases: new string[] { "--serialPort" },
                    getDefaultValue: () => null,
                    description: "A serial port for use with serial TNCs."),
                new Option(
                    aliases: new string[] { "--display-parse-failures" },
                    description: "If specified, prints packets that failed to parse. Helpful for development."
                ),
                };
#pragma warning restore CA1861 // Avoid constant arrays as arguments
            rootCommand.Name = "APRSsharp";
            rootCommand.Description = $@"AprsSharp Console App
                Version: {Utilities.GetAssemblyVersion()}
                GitHub: https://github.com/CBielstein/APRSsharp
                Author: Cameron Bielstein";

            // The parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler
                .Create(async (Mode mode, string callsign, string password, string server, int port, string filter, bool displayUnsupported, string? serialPort, bool displayParseFailures)
                        => await Execute(mode, callsign, password, server, port, filter, displayUnsupported, serialPort, displayParseFailures));

            rootCommand.Invoke(args);
        }

        /// <summary>
        /// Executes the logic for running in TNC mode.
        /// </summary>
        /// <param name="tnc">A <see cref="Tnc"/> with which to interface.</param>
        /// <param name="callsign">The callsign to use for sent messages.</param>
        /// <param name="displayParseFailures">Whether or not to print message parse failures to the command line.</param>
        private static void RunTncMode(Tnc tnc, string callsign, bool displayParseFailures)
        {
            tnc.FrameReceivedEvent += (sender, args) =>
            {
                var byteArray = args.Data.ToArray();

#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    var packet = new Packet(byteArray);
                    PrintPacket(packet);
                }
                catch (Exception ex)
                {
                    if (displayParseFailures)
                    {
                        PrintParseFailure(ex, Encoding.ASCII.GetString(byteArray));
                    }
                }
#pragma warning restore CA1031 // Do not catch general exception types
            };

            tnc.SetTxDelay(50);
            tnc.SetTxTail(50);

            Console.WriteLine("Enter status to send, else q to quit");

            do
            {
                var input = Console.ReadLine();
                if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else if (string.Equals(callsign, "N0CALL", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("You must supply your callsign to send packets.");
                }
                else
                {
                    var packet = new Packet(callsign, callsign, new List<string>(), new StatusInfo((Timestamp?)null, input));
                    tnc.SendData(packet.EncodeAx25());
                }
            }
            while (true);
        }

        /// <summary>
        /// Prints a parse failure to the user.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> that occurred.</param>
        /// <param name="attempted">The <see cref="string"/> that failed to decode.</param>
        private static void PrintParseFailure(Exception ex, string attempted)
        {
            Console.WriteLine("Failed to decode.");
            Console.WriteLine($"    Packet: {attempted}");
            Console.WriteLine($"    Exception: {ex}");
        }

        /// <summary>
        /// Executes functionality using the provided command line arguments.
        /// </summary>
        /// <param name="mode">The mode of operation for this invocation of the program.</param>
        /// <param name="callsign">The user callsign that they should input.</param>
        /// <param name="password">The user password.</param>
        /// <param name="server">The specified server to connect (either APRS-IS or TCP TNC).</param>
        /// <param name="port">A port to use for connection to a TNC via TCP.</param>
        /// <param name="filter">The filter that will be used for receiving the packets.
        /// This parameter shouldn't include the `filter` at the start, just the logic string itself.</param>
        /// <param name="displayUnsupported">If true, display packets with unsupported info field types. If false, such packets are not displayed.</param>
        /// <param name="serialPort">A serial port to use for connection to a TNC via serial connection.</param>
        /// <param name="displayParseFailures">Whether to print parse failures.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private static async Task Execute(
            Mode mode,
            string callsign,
            string password,
            string server,
            int port,
            string filter,
            bool displayUnsupported,
            string? serialPort,
            bool displayParseFailures)
        {
            Program.displayUnsupported = displayUnsupported;

            switch (mode)
            {
                case Mode.APRSIS:
                {
                    Console.WriteLine($"Connecting to APRS-IS server: {server}");

                    using AprsIsClient n = new AprsIsClient();
                    n.ReceivedPacket += PrintPacket;

                    if (displayParseFailures)
                    {
                        n.DecodeFailed += PrintParseFailure;
                    }

                    Task receive = n.Receive(callsign, password, server, filter);

                    while (true)
                    {
                        ConsoleKeyInfo input = Console.ReadKey();

                        if (input.Key == ConsoleKey.Q)
                        {
                            n.Disconnect();
                            await receive;
                            break;
                        }
                    }

                    break;
                }

                case Mode.TCPTNC:
                {
                    Console.WriteLine($"Connecting to KISS TNC via TCP: {server}:{port}");

                    using TcpConnection tcp = new TcpConnection();
                    tcp.Connect(server, port);
                    using Tnc tnc = new TcpTnc(tcp, 0);

                    RunTncMode(tnc, callsign, displayParseFailures);

                    break;
                }

                case Mode.SERIALTNC:
                {
                    if (serialPort == null)
                    {
                        Console.WriteLine("You must specify a serial port to use serial TNC mode.");
                        return;
                    }

                    Console.WriteLine($"Connecting to KISS TNC via serial: {serialPort}");

                    using SerialConnection serial = new SerialConnection(serialPort);
                    using Tnc tnc = new SerialTnc(serial, 0);

                    RunTncMode(tnc, callsign, displayParseFailures);

                    break;
                }

                default:
                    throw new ArgumentException($"Unsupported execution mode: {mode}");
            }
        }
    }
}
