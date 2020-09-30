namespace AprsSharp.Applications.Console
{
    using System;
    using AprsSharp.Parsers.Aprs;

    /// <summary>
    /// The public class that will be called when building the console application.
    /// It is the main class that will have functionality of calling and decoding the packets.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method that takes in raw packet strings.
        /// </summary>
        /// <param name="args"> The input arguments for the program i.e packets which will be strings.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter the packet name ");
            var packetName = Console.ReadLine();
            Packet p = new Packet();
            p.DecodeInformationField(packetName);
            Timestamp ts = (Timestamp)p.timestamp;
            Position pos = (Position)p.position;
            Console.WriteLine($"\nHello, your packet name is, {p.comment}, at cordinates, {pos.Coordinates}, and time, {ts.dateTime.Hour}");
         }
    }
}
