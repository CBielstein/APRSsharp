namespace AprsSharp.Applications.Console
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using AprsSharp.Connections.AprsIs;

    /// <summary>
    /// The public class that will be called when building the console application.
    /// It is the main class that will have functionality of calling and decoding the packets.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method that takes in raw packet strings.
        /// </summary>
        /// <param name="args"> Parameters to be passed as commandline arguments.</param>
        public static void Main(string[] args)
        {
            // Create a root command with some options
            var rootCommand = new RootCommand
                {
                new Option<string>(
                    aliases: new string[] { "--callsign", "-c", "--cgn" },
                    getDefaultValue: () => AprsIsConnection.AprsIsConstants.DefaultCallsign,
                    description: "A user callsign parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--password", "-p", "--pwd", "--pass" },
                    getDefaultValue: () => AprsIsConnection.AprsIsConstants.DefaultPassword,
                    description: "A user password whose argument is parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--server", "-s", "--svr" },
                    getDefaultValue: () => AprsIsConnection.AprsIsConstants.DefaultServerName,
                    description: "A specified server parsed as a string"),
                new Option<string>(
                    aliases: new string[] { "--filter", "-f" },
                    getDefaultValue: () => AprsIsConnection.AprsIsConstants.DefaultFilter,
                    description: "A user filter parsed as a string"),
                };
            rootCommand.Description = "AprsSharp Console App";

            using var cli = new AprsSharpCli();

            // The parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string, string, string, string, IConsole>(cli.Execute);

            rootCommand.Invoke(args);
        }
    }
}
