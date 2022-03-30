namespace AprsSharp.Applications.Console
{
    using System;
    using System.CommandLine;

    /// <summary>
    /// Extension methods for <see cref="IConsole"/>.
    /// </summary>
    public static class IConsoleExtensions
    {
        /// <summary>
        /// Writes a line followed by a newline character.
        /// </summary>
        /// <param name="console">The <see cref="IConsole"/> to which to write.</param>
        /// <param name="message">The message to print.</param>
        public static void WriteLine(this IConsole console, string? message = null)
        {
            console.Out.Write($"{message ?? string.Empty}{Environment.NewLine}");
        }
    }
}
