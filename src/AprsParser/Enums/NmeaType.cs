namespace AprsSharp.Parsers.Aprs
{
    /// <summary>
    /// Used to specify format during decode of raw GPS data packets.
    /// </summary>
    public enum NmeaType
    {
        /// <summary>
        /// Not yet decoded
        /// </summary>
        NotDecoded,

        /// <summary>
        /// GGA: Global Position System Fix Data
        /// </summary>
        GGA,

        /// <summary>
        /// GLL: Geographic Position, Latitude/Longitude Data
        /// </summary>
        GLL,

        /// <summary>
        /// RMC: Recommended Minimum Specific GPS/Transit Data
        /// </summary>
        RMC,

        /// <summary>
        /// VTG: Velocity and Track Data
        /// </summary>
        VTG,

        /// <summary>
        /// WPT: Way Point Location (also WPL)
        /// </summary>
        WPT,

        /// <summary>
        /// Not supported/known type
        /// </summary>
        Unknown,
    }
}
