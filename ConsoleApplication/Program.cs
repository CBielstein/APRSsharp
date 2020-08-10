using System;
using AprsSharp.Parsers.Aprs;

namespace ConsoleApplication.Aprs
{
    class Program
    {
        static void Main(string[] args)
        {     
          
            Console.WriteLine("Enter the packet name ");
            var packetName = Console.ReadLine();

            Packet p = new Packet();      
            p.DecodeInformationField (packetName);  

            Timestamp ts = (Timestamp)p.timestamp;
            Position pos = (Position)p.position;

            Console.WriteLine($"\nHello, your packet name is, {p.comment}, at cordinates, {pos.Coordinates}, and time, {ts.dateTime.Hour}");
                           
        }
    }
}
