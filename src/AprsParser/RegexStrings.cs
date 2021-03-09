namespace AprsSharp.Parsers.Aprs
{
    /// <summary>
    /// Holds strings for regex comparisons for reuse.
    /// </summary>
    internal static class RegexStrings
    {
        /// <summary>
        /// Matches Maidenhead grid with optional symbols.
        /// Three groups: Full, alphanumber grid, symbols.
        /// </summary>
        public const string MaidenheadGridWithOptionalSymbols = @"([a-zA-Z0-9]{4,8})(.{2})?";

        /// <summary>
        /// Same as <see cref="MaidenheadGridWithOptionalSymbols"/> but forces full line match.
        /// </summary>
        /// <value></value>
        public static readonly string MaidenheadGridFullLine = $@"^{MaidenheadGridWithOptionalSymbols}$";

        /// <summary>
        /// Matches a Miadenhead Grid Locator Beacon with comment.
        /// Four groups: Full, alphanumeric grid, symbols, comment.
        /// </summary>
        public static readonly string MaidenheadGridLocatorBeacon = $@"^\[{MaidenheadGridWithOptionalSymbols}\](.+)?$";
    }
}