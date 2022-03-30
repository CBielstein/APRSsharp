namespace AprsSharp.Applications.Console
{
    using System;
    using System.CommandLine;
    using System.Threading;
    using AprsSharp.Connections.AprsIs;
    using AprsSharp.Parsers.Aprs;

    /// <summary>
    /// The primary logic of the AprsSharp CLI.
    /// </summary>
    public sealed class AprsSharpCli : IDisposable
    {
        private string callsign = null!;
        private string password = null!;
        private string server = null!;
        private string filter = null!;
        private IConsole console = null!;
        private TcpConnection? tcpConn;
        private AprsIsConnection isConn = null!;
        private bool disposed;

        /// <summary>
        /// Begins execution of the AprsIsCli logic.
        /// </summary>
        /// <param name="callsign">Connection callsign.</param>
        /// <param name="password">Connection password.</param>
        /// <param name="server">Server for connection.</param>
        /// <param name="filter">Packet filter string.</param>
        /// <param name="console">Console for writing.</param>
        public void Execute(string callsign, string password, string server, string filter, IConsole console)
        {
            this.callsign = callsign;
            this.password = password;
            this.server = server;
            this.filter = filter;
            this.console = console;
            ReInit();

            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            tcpConn?.Dispose();
        }

        private void ReInit()
        {
            tcpConn?.Dispose();
            tcpConn = new TcpConnection();
            isConn = new AprsIsConnection(tcpConn);
            isConn.ReceivedPacket += PrintPacket;
            isConn.ChangedState += StateChange;
            _ = isConn.Receive(callsign, password, server, filter);
        }

        /// <summary>
        /// Handles a <see cref="AprsIsConnection.ChangedState"/> by recreating the TCP and APRS-IS connections
        /// when the state becomes <see cref="ConnectionState.Disconnected"/>.
        /// </summary>
        /// <param name="state">New state of the <see cref="AprsIsConnection"/>.</param>
        private void StateChange(ConnectionState state)
        {
            if (state == ConnectionState.Disconnected)
            {
                Thread.Sleep(5000);
                ReInit();
            }
        }

        /// <summary>
        /// A function matching the delegate event to print the received packet.
        /// </summary>
        /// <param name="p">A <see cref="Packet"/> to be printed.</param>
        private void PrintPacket(Packet p)
        {
            console.WriteLine();
            console.WriteLine($"Received type: {p.InfoField.Type}");

            console.WriteLine($"    Sender: {p.Sender}");
            console.WriteLine($"    Path: {string.Join(',', p.Path)}");
            console.WriteLine($"    Received At: {p.ReceivedTime} {p.ReceivedTime?.Kind}");
            console.WriteLine($"    Type: {p.InfoField.Type}");

            // TODO Issue #103: Reduce copy/paste below
            // TODO Issue #103: Clean up position printing:
                // * Position lat/long encoding uses symbol IDs, not the most user-friendly
                // * Gridsquare print out should probably print the correct number of characters based on ambiguitiy
            if (p.InfoField is PositionInfo pi)
            {
                console.WriteLine($"    Timestamp: {pi.Timestamp?.DateTime} {pi.Timestamp?.DateTime.Kind}");
                console.WriteLine($"    Position: {pi.Position.Encode()} ({pi.Position.EncodeGridsquare(4, false)})");
                console.WriteLine($"    Comment: {pi.Comment}");
                console.WriteLine($"    Has Messaging: {pi.HasMessaging}");

                if (p.InfoField is WeatherInfo wi)
                {
                    console.WriteLine($"Wind direction (degrees): {wi.WindDirection}");
                    console.WriteLine($"Wind speed (one-minute sustained): {wi.WindSpeed}");
                    console.WriteLine($"Wind gust (5 minute max, mph): {wi.WindGust}");
                    console.WriteLine($"Temperature (F): {wi.Temperature}");
                    console.WriteLine($"1-hour rainfall (100th of inch): {wi.Rainfall1Hour}");
                    console.WriteLine($"24-hour rainfall (100th of inch): {wi.Rainfall24Hour}");
                    console.WriteLine($"Rainfall since midnight (100th of inch): {wi.RainfallSinceMidnight}");
                    console.WriteLine($"Humidity: {wi.Humidity}");
                    console.WriteLine($"Barometric pressure: {wi.BarometricPressure}");
                    console.WriteLine($"Luminosity: {wi.Luminosity}");
                    console.WriteLine($"Raw rain: {wi.RainRaw}");
                    console.WriteLine($"Snow (inches, last 24 hours): {wi.Snow}");
                }
            }
            else if (p.InfoField is StatusInfo si)
            {
                console.WriteLine($"    Timestamp: {si.Timestamp?.DateTime} {si.Timestamp?.DateTime.Kind}");
                console.WriteLine($"    Position: {si.Position?.Encode()} ({si.Position?.EncodeGridsquare(4, false)})");
                console.WriteLine($"    Comment: {si.Comment}");
            }
            else if (p.InfoField is MaidenheadBeaconInfo mbi)
            {
                console.WriteLine($"    Position: {mbi.Position.EncodeGridsquare(4, false)}");
                console.WriteLine($"    Comment: {mbi.Comment}");
            }

            console.WriteLine();
        }
    }
}
