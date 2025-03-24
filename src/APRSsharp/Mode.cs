namespace AprsSharp.Applications.Console;

/// <summary>
/// The modes in which this program can operate.
/// </summary>
public enum Mode
{
    /// <summary>
    /// Receives packets from APRS-IS
    /// </summary>
    APRSIS,

    /// <summary>
    /// Interfaces with a TNC via TCP
    /// </summary>
    TCPTNC,
}
