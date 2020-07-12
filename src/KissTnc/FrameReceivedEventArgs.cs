namespace AprsSharp.Protocols
{
    using System;

    /// <summary>
    /// Caries the bytes delivered in a single frame by the TNC.
    /// </summary>
    public class FrameReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="bytes">Data bytes received in the frame.</param>
        public FrameReceivedEventArgs(byte[] bytes)
        {
            Data = bytes;
        }

        /// <summary>
        /// Gets the data bytes of the received frame.
        /// </summary>
        public byte[] Data
        {
            get;
            private set;
        }
    }
}
