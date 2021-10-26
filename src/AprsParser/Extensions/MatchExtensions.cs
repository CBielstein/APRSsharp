namespace AprsSharp.Parsers.Aprs.Extensions
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extension methods to encourage code reuse in this codebase.
    /// </summary>
    public static class MatchExtensions
    {
        /// <summary>
        /// Asserts success of a Regex.Match call.
        /// If the Match object is not successful, throws an ArgumentException.
        /// </summary>
        /// <param name="match">Match object to check for success.</param>
        /// <param name="type">Type that was being checked against the regex.</param>
        /// <param name="paramName">The name of the param that did not match the regex.</param>
        public static void AssertSuccess(this Match match, string type, string paramName)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            else if (!match.Success)
            {
                throw new ArgumentException($"{type} did not match regex", paramName);
            }
        }

        /// <summary>
        /// Asserts success of a Regex.Match call.
        /// If the Match object is not successful, throws an ArgumentException.
        /// </summary>
        /// <param name="match">Match object to check for success.</param>
        /// <param name="type">Type that was being checked against the regex.</param>
        /// <param name="paramName">The name of the param that did not match the regex.</param>
        public static void AssertSuccess(this Match match, Packet.Type type, string paramName)
        {
            match.AssertSuccess(type.ToString(), paramName);
        }
    }
}
