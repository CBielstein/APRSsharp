namespace AprsSharp.Protocols.KISS;

using System;
using System.IO.Ports;

/// <summary>
/// Abstracts a serial port connection.
/// </summary>
public interface ISerialConnection : IDisposable
{
    /// <summary>
    /// Indicates that data has been received through the <see cref="ISerialConnection"/>.
    /// </summary>
    event SerialDataReceivedEventHandler DataReceived;

    /// <summary>
    /// Gets a value indicating whether the <see cref="ISerialConnection"/> is open or closed.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Opens a new serial port connection.
    /// </summary>
    void Open();

    /// <summary>
    /// Writes a specified number of bytes to the serial port using data from a buffer.
    /// </summary>
    /// <param name="buffer"> The byte array that contains the data to write to the port.</param>
    /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
    /// <param name="count">The number of bytes to write.</param>
    void Write(byte[] buffer, int offset, int count);
}
