namespace AprsSharp.KissTnc;

using System.IO.Ports;

/// <summary>
/// Wraps a <see cref="SerialPort"/> object.
/// </summary>
public sealed class SerialConnection : ISerialConnection
{
    private readonly SerialPort serialPort;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerialConnection"/> class.
    /// </summary>
    /// <param name="portName">The name of the <see cref="SerialPort"/> to open.</param>
    public SerialConnection(string portName)
    {
        serialPort = new SerialPort(portName);
    }

    /// <inheritdoc/>
    public event SerialDataReceivedEventHandler DataReceived
    {
        add => serialPort.DataReceived += value;
        remove => serialPort.DataReceived -= value;
    }

    /// <inheritdoc/>
    public bool IsOpen => serialPort.IsOpen;

    /// <inheritdoc/>
    public void Dispose()
    {
        serialPort.Dispose();
    }

    /// <inheritdoc/>
    public void Open() => serialPort.Open();

    /// <inheritdoc/>
    public void Write(byte[] buffer, int offset, int count) => serialPort.Write(buffer, offset, count);
}
