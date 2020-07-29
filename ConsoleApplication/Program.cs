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
      
            Console.WriteLine($"\nHello, your packet is , {packetName}");
                           
        }
    }
}
