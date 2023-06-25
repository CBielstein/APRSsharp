namespace AprsSharp.Parsers.Aprs
{
    /// <summary>
    /// Holds strings for regex comparisons for reuse.
    /// </summary>
    internal static class RegexStrings
    {
        /// <summary>
        /// Same as <see cref="MaidenheadGridWithSymbols"/> but symbols are optional.
        /// Three groups: Full, alphanumeric grid, symbols.
        /// </summary>
        public const string MaidenheadGridWithOptionalSymbols = $@"{MaidenheadGridWithSymbols}?";

        /// <summary>
        /// Matches Maidenhead grid with symbols.
        /// Three groups: Full, alphanumeric grid, symbols.
        /// </summary>
        public const string MaidenheadGridWithSymbols = @"([a-zA-Z0-9]{4,8})(.{2})";

        /// <summary>
        /// Matches Maidenhead grid of 4-6 characters without symbols.
        /// One group: Full match.
        /// </summary>
        public const string MaidenheadGrid4or6NoSymbols = @"([a-zA-Z0-9]{4,6})";

        /// <summary>
        /// Matchdes a lat/long position, including with ambiguity, including symbols.
        /// Five matches:
        ///     Full
        ///     Latitute
        ///     Symbol table ID
        ///     Longitude
        ///     Symbol Code.
        /// </summary>
        public const string PositionLatLongWithSymbols = @"([0-9 \.NS]{8})(.)([0-9 \.EW]{9})(.)";

        /// <summary>
        /// Same as <see cref="MaidenheadGridWithOptionalSymbols"/> but forces full line match.
        /// </summary>
        /// <value></value>
        public const string MaidenheadGridFullLine = $@"^{MaidenheadGridWithOptionalSymbols}$";

        /// <summary>
        /// Matches a Miadenhead Grid in square brackets [] with comment.
        /// Four groups: Full, alphanumeric grid, comment.
        /// </summary>
        public const string MaidenheadGridLocatorBeacon = $@"^\[{MaidenheadGrid4or6NoSymbols}\](.+)?$";

        /// <summary>
        /// Matches a Status info field with Maidenhead grid and optional comment (comment separated by a space)
        /// Four matches: Full, full maidenhead, alphanumeric grid, symbols, comment.
        /// </summary>
        public const string StatusWithMaidenheadAndComment = $@"^>({MaidenheadGridWithSymbols}) ?(.+)?$";

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
        public const string PositionWithoutTimestamp = $@"^[!=]({PositionLatLongWithSymbols})(.+)?$";

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
        public const string PositionWithTimestamp = $@"^([/@])([0-9]{{6}}[/zh0-9])({PositionLatLongWithSymbols})(.+)?$";

        /// <summary>
        /// Matches a full TNC2-encoded packet.
        /// 4 matches:
        ///     Full
        ///     Sender callsign
        ///     Path
        ///     Info field.
        /// </summary>
        public const string Tnc2Packet = @"^([^>]+)>([^:]+):(.+)$";

        /// <summary>
        /// Validates that a given string is only alphanumeric characters in a single line.
        /// </summary>
        public const string Alphanumeric = @"^[a-zA-Z0-9]+$";
    }
}
