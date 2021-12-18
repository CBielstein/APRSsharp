namespace AprsSharp.Parsers.Aprs
{
    /// <summary>
    /// Specifies whether a function is referring to latitude or longitude during Maidenhead gridsquare encode/decode.
    /// </summary>
    public enum CoordinateSystem
    {
        /// <summary>
        /// Latitute.
        /// </summary>
        Latitude,

        /// <summary>
        /// Longitude.
        /// </summary>
        Longitude,
    }
}