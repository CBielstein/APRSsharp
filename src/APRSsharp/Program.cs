namespace AprsSharp.Applications.Console
{
    using System;
    using System.Threading;
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
        /// <param name="tcpMessage">The received tcp message that needs to be printed.</param>
        public static void PrintTcpMessage(string tcpMessage)
        {
            Console.WriteLine(tcpMessage);
        }

        /// <summary>
        /// Main method that takes in raw packet strings.
        /// </summary>
        /// <param name="args"> The input arguments for the program i.e packets which will be strings.</param>
        /// <returns>Returns an async task that can be stored.</returns>
        public static async Task Main(string[] args)
        {
            using TcpConnection tcpConnection = new TcpConnection();
            AprsIsConnection n = new AprsIsConnection(tcpConnection);
            string callsign;
            string password;

            // get input from the user
            Console.WriteLine("Enter your callsign: ");
            callsign = Console.ReadLine();
            Console.WriteLine("Enter your password: ");
            password = Console.ReadLine();
            Console.WriteLine("Press key Q or key q to cancel the connection:\n ");
            string? callsignArg = string.IsNullOrEmpty(callsign) ? null : callsign;
            string? passwordArg = string.IsNullOrEmpty(password) ? null : password;
            n.ReceivedTcpMessage += PrintTcpMessage;
            Task receive = n.Receive(callsignArg, passwordArg);

            ConsoleKeyInfo input;
            input = Console.ReadKey();

            while (true)
            {
                if (input.Key == ConsoleKey.Q)
                {
                    n.Disconnect();
                    await receive;
                    break;
                }
                else
                {
                   input = Console.ReadKey();
                }
            }
         }
    }
}
