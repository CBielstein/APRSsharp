namespace AprsSharp.Parsers.Aprs
{
    using System;
    using AprsSharp.Parsers.Aprs.Extensions;

    /// <summary>
    /// A representation of an info field on an APRS packet.
    /// </summary>
    public abstract class InfoField
    {
        /// <summary>
        /// Gets or sets the <see cref="PacketType"/> of this packet.
        /// </summary>
        public PacketType Type { get; protected set; }

        /// <summary>
        /// Instantiates a type of <see cref="InfoField"/> from the given string.
        /// </summary>
        /// <param name="encodedInfoField">String representation of the APRS info field.</param>
        /// <returns>A class extending <see cref="InfoField"/>.</returns>
        public static InfoField FromString(string encodedInfoField)
        {
            PacketType type = GetPacketType(encodedInfoField);

            switch (type)
            {
                case PacketType.PositionWithoutTimestampNoMessaging:
                case PacketType.PositionWithoutTimestampWithMessaging:
                case PacketType.PositionWithTimestampNoMessaging:
                case PacketType.PositionWithTimestampWithMessaging:
                    return new PositionInfo(encodedInfoField);

                case PacketType.Status:
                    return new StatusInfo(encodedInfoField);

                case PacketType.MaidenheadGridLocatorBeacon:
                    return new MaidenheadBeaconInfo(encodedInfoField);

                default:
                    throw new NotImplementedException($"FromString not implemented for info field type {type}");
            }
        }

        /// <summary>
        /// Encodes an APRS info field to a string.
        /// </summary>
        /// <returns>String representation of the packet.</returns>
        public abstract string Encode();

        /// <summary>
        /// Gets the <see cref="PacketType"/> of a string representation of an APRS info field.
        /// </summary>
        /// <param name="encodedInfoField">A string-encoded APRS info field.</param>
        /// <returns><see cref="PacketType"/> of the info field.</returns>
        protected static PacketType GetPacketType(string encodedInfoField)
        {
            if (encodedInfoField == null)
            {
                throw new ArgumentNullException(nameof(encodedInfoField));
            }

            // TODO Issue #67: This isn't always true.
            // '!' can come up to the 40th position.
            char dataTypeIdentifier = char.ToUpperInvariant(encodedInfoField[0]);

            return dataTypeIdentifier.ToPacketType();
        }
    }
}
