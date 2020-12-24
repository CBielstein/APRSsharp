﻿namespace AprsSharp.Applications.Console
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
            string callsign;
            string password;

            // get input from the user
            Console.WriteLine("Enter your callsign: ");
            callsign = Console.ReadLine();
            Console.WriteLine("Enter your password: ");
            password = Console.ReadLine();
            string? callsignArg = string.IsNullOrEmpty(callsign) ? null : callsign;
            string? passwordArg = string.IsNullOrEmpty(password) ? null : password;
            n.ReceivedTcpMessage += PrintTcpMessage;
            n.Receive(callsignArg, passwordArg);

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
