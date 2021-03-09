namespace AprsSharp.Parsers.Aprs
{
    /// <summary>
    /// Holds strings for regex comparisons for reuse.
    /// </summary>
    internal static class RegexStrings
    {
        /// <summary>
        /// Matches a Miadenhead Grid Locator Beacon with comment.
        /// </summary>
        public const string MaidenheadGridLocatorBeacon = @"^\[(.{4,6})\](.+)?$";

        /// <summary>
        /// Matches Maidenhead grid with optional symbols, full line match.
        /// </summary>
        public const string MaidenheadGridWithOptionalSymbols = @"^([a-zA-Z0-9]{4,8})(.{2})?$";
    }
}