using System;
using System.Collections.Generic;
using System.Text;

namespace KissIt
{
    /// <summary>
    /// Represents special characters in the KISS protocol
    /// </summary>
    public enum KISSSpecialCharacters
    {
        /// <summary>
        /// Frame End
        /// </summary>
        FEND = 0xC0,

        /// <summary>
        /// Frame Escape
        /// </summary>
        FESC = 0xDB,

        /// <summary>
        /// Transposed Frame End
        /// </summary>
        TFEND = 0xDC,

        /// <summary>
        /// Transposed Frame Escape
        /// </summary>
        TFESC = 0xDD,
    }
}
