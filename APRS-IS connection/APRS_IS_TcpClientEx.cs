namespace AprsSharp.Applications.APRS_IS_Connection
{
 using System;
 using System.IO;
 using System.Net.Sockets;
 using System.Text;

 /// <summary>
 /// This is the APRS IS TCPClient class that establishes a connection to the APRS-IS server.
 /// The program shows any packets flowing through.
 /// </summary>
 public class APRS_IS_TcpClientEx
 {
   /// <summary>
   /// This is the main method (entry point of the program) where the program control starts and ends.
   /// </summary>
    public static void Main()
    {
      try
      {
        using var client = new TcpClient();

        // list of servers in the world is found here http://www.aprs2.net/ and http://status.aprs2.net/

        // var hostname = "euro.aprs2.net";// this is the server for europe and africa

        // var hostname = "noam.aprs2.net"; //this is the server for north america
        var hostname = "rotate.aprs2.net";
        client.Connect(hostname, 14580); // Alternative port number 10152
        using NetworkStream networkStream = client.GetStream();
        networkStream.ReadTimeout = 200000;
        using var writer = new StreamWriter(networkStream);

        // var message = $"user 0 pass -1 vers 0.0 filter r/33.25/-96.5/50";

        // var message = "user 0 pass -1 filter d/400";

        // filter parameters for the different aprs http://www.aprs-is.net/javAPRSFilter.aspx
        var message = "user 0 pass -1";
        Console.WriteLine(message);
        using var reader = new StreamReader(networkStream, System.Text.Encoding.ASCII);
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(message);
        networkStream.Write(bytes, 0, bytes.Length);
        Console.WriteLine(reader.ReadToEnd());
        networkStream.Close();
        client.Close();
      }
      catch (ArgumentNullException e)
      {
        Console.WriteLine("ArgumentNullException: {0}", e);
      }
      catch (SocketException e)
      {
      Console.WriteLine("SocketException: {0}", e);
      }

      Console.WriteLine("\n Press Enter to continue...");
      Console.Read();
    }
  }
}