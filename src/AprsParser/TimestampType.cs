namespace AprsSharp.Parsers.Aprs
{
        /// <summary>
        /// Represents the type of APRS timestamp.
        /// </summary>
        public enum TimestampType
        {
            /// <summary>
            /// Days/Hours/Minutes zulu
            /// </summary>
            DHMz,

            /// <summary>
            /// Days/Hours/Minutes local
            /// </summary>
            DHMl,

            /// <summary>
            /// Hours/Minutes/Seconds (always zulu)
            /// </summary>
            HMS,

            /// <summary>
            /// Hours/Minutes/Seconds (always zulu)
            /// </summary>
            MDHM,

            /// <summary>
            /// Not a decoded timestamp
            /// </summary>
            NotDecoded,
        }

}