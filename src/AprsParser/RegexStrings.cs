namespace AprsSharp.Parsers.Aprs
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Holds strings for regex comparisons for reuse.
    /// </summary>
    internal static class RegexStrings
    {
        /// <summary>
        /// Matches Maidenhead grid with optional symbols.
        /// Three groups: Full, alphanumeric grid, symbols.
        /// </summary>
        public const string MaidenheadGridWithOptionalSymbols = @"([a-zA-Z0-9]{4,8})(.{2})?";

        /// <summary>
        /// Matchdes a lat/long position, including with ambiguity, including symbols.
        /// Five matches:
        ///     Full
        ///     Latitute
        ///     Symbol table ID
        ///     Longitude
        ///     Symbol Code.
        /// </summary>
        public const string PositionLatLongWithSymbols = @"([0-9 \.NW]{8})(.)([0-9 \.EW]{9})(.)";

        /// <summary>
        /// Same as <see cref="MaidenheadGridWithOptionalSymbols"/> but forces full line match.
        /// </summary>
        /// <value></value>
        public static readonly string MaidenheadGridFullLine = $@"^{MaidenheadGridWithOptionalSymbols}$";

        /// <summary>
        /// Matches a Miadenhead Grid in square brackets [] with comment.
        /// Four groups: Full, alphanumeric grid, symbols, comment.
        /// </summary>
        public static readonly string MaidenheadGridLocatorBeacon = $@"^\[{MaidenheadGridWithOptionalSymbols}\](.+)?$";

        /// <summary>
        /// Matches a Status info field with Maidenhead grid and optional comment (comment separated by a space)
        /// Four matches: Full, full maidenhead, alphanumeric grid, symbols, comment.
        /// </summary>
        public static readonly string StatusWithMaidenheadAndComment = $@"^>({MaidenheadGridWithOptionalSymbols}) ?(.+)?$";

        /// <summary>
        /// Matches a PositionWithoutTimestamp info field.
        /// Seven mathces:
        ///     Full
        ///     Full lat/long coords and symbols
        ///     Latitude
        ///     Symbol table
        ///     Longitude
        ///     Symbol code
        ///     Optional comment.
        /// </summary>
        public static readonly string PositionWithoutTimestamp = $@"^!({PositionLatLongWithSymbols})(.+)?$";

        /// <summary>
        /// Matches a PositionWithTimestamp info field.
        /// 9 mathces:
        ///     Full
        ///     Packet type symbol (/ or @)
        ///     Timestamp
        ///     Full position
        ///         Latitude
        ///         Symbol table
        ///         Longitude
        ///         Symbol code
        ///     Optional comment.
        /// </summary>
        public static readonly string PositionWithTimestamp = $@"^([/@])([0-9]{{6}}[/zh0-9])({PositionLatLongWithSymbols})(.+)?$";
    }
}
