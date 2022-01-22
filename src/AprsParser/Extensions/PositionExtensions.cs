namespace AprsSharp.Parsers.Aprs.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for <see cref="Position"/>.
    /// </summary>
    public static class PositionExtensions
    {
        /// <summary>
        /// Determines if a <see cref="Position"/>'s symbol is a weather station.
        /// </summary>
        /// <param name="position">A <see cref="Position"/> to check.</param>
        /// <returns>True if the <see cref="Position"/>'s symbol is a weather station, else false.</returns>
        public static bool IsWeatherSymbol(this Position position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            return (position.SymbolTableIdentifier == '/' || position.SymbolTableIdentifier == '\\') && position.SymbolCode == '_';
        }
    }
}
