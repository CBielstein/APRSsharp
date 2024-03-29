namespace AprsSharp.KissTnc
{
    /// <summary>
    /// Represents special characters in the KISS protocol.
    /// All values and descriptions taken from KA9Q's KISS paper.
    /// http://www.ka9q.net/papers/kiss.html.
    /// </summary>
    public enum SpecialCharacter
    {
        /// <summary>
        /// Frame End
        /// </summary>
        FEND = 0xC0,

        /// <summary>
        /// Frame Escape
        /// </summary>
        FESC = 0xDB,

        /// <summary>
        /// Transposed Frame End
        /// </summary>
        TFEND = 0xDC,

        /// <summary>
        /// Transposed Frame Escape
        /// </summary>
        TFESC = 0xDD,
    }

    /// <summary>
    /// Represents codes used to command the TNC in the KISS protocol.
    /// These are embedded in the encoding of frames sent to the TNC.
    /// </summary>
    public enum Command
    {
        /// <summary>
        /// The rest of the frame is data to be sent on the HDLC channel.
        /// </summary>
        DataFrame = 0,

        /// <summary>
        /// The next byte is the transmitter keyup delay in 10 ms units.
        /// The default start-up value is 50 (i.e., 500 ms).
        /// </summary>
        TxDelay = 1,

        /// <summary>
        /// The next byte is the persistence parameter, p, scaled to the
        /// range of 0 - 255 with the folloing formula: P = p * 256 - 1
        /// The default value is P = 63 (i.e. p = 0.25).
        /// </summary>
        P = 2,

        /// <summary>
        /// The next byte is the slot interval in 10 ms units.
        /// The default is 10 (i.e. 100ms).
        /// </summary>
        SlotTime = 3,

        /// <summary>
        /// The next byte is the time to hold up the TX after the FCS has been sent,
        /// in 10 MS units. This command is obsolete, and is included here only for
        /// compatibility with some existing implementations.
        /// </summary>
        TxTail = 4,

        /// <summary>
        /// The next byte is 0 for half duplex, nonzero for full duplex.
        /// The default is 0 (i.e., half duplex).
        /// </summary>
        FullDuplex = 5,

        /// <summary>
        /// Specific for each TNC. In the TNC-1, this command sets the modem speed.
        /// Other implementations may use this function for other hardware-specific functions.
        /// </summary>
        SetHardware = 6,

        /// <summary>
        /// Exit KISS and return control to a higher-level program. This is useful
        /// only when KISS is incorporated in to the TNC along with other applications.
        /// </summary>
        RETURN = 0xFF,
    }

    /// <summary>
    /// Used to set duplex mode.
    /// </summary>
    public enum DuplexState
    {
        /// <summary>
        /// Half duplex
        /// </summary>
        HalfDuplex,

        /// <summary>
        /// Full duplex
        /// </summary>
        FullDuplex,
    }
}
