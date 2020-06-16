using System;

namespace KissIt
{
    /// <summary>
    /// Caries the bytes delivered in a single frame by the TNC
    /// </summary>
    public class FrameReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new FrameReceivedEventArgs object
        /// </summary>
        /// <param name="bytes">Data bytes received in the frame</param>
        public FrameReceivedEventArgs(byte[] bytes)
        {
            Data = bytes;
        }

        /// <summary>
        /// The data bytes of the received frame
        /// </summary>
        public byte[] Data
        {
            get;
            private set;
        }
    }
}
