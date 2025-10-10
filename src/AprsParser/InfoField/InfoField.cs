namespace AprsSharp.AprsParser
{
    using System;
    using System.Data.SqlTypes;
    using System.Text;
    using AprsSharp.AprsParser.Extensions;

    /// <summary>
    /// A representation of an info field on an APRS packet.
    /// </summary>
    public abstract class InfoField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoField"/> class.
        /// </summary>
        public InfoField()
        {
            Type = PacketType.Unknown;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoField"/> class from an encoded string.
        /// </summary>
        /// <param name="encodedInfoField">An encoded InfoField from which to pull the Type.</param>
        public InfoField(string encodedInfoField)
        {
            if (encodedInfoField == null)
            {
                throw new ArgumentNullException(nameof(encodedInfoField));
            }

            Type = GetPacketType(encodedInfoField);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoField"/> class from a <see cref="PacketType"/>.
        /// </summary>
        /// <param name="type">The <see cref="PacketType"/> of this <see cref="InfoField"/>.</param>
        public InfoField(PacketType type)
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoField"/> class from another <see cref="InfoField"/>.
        /// This is the copy constructor.
        /// </summary>
        /// <param name="infoField">An <see cref="InfoField"/> to copy.</param>
        public InfoField(InfoField infoField)
        {
            if (infoField == null)
            {
                throw new ArgumentNullException(nameof(infoField));
            }

            Type = infoField.Type;
        }

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

            if (type.IsMicEType())
            {
                throw new NotImplementedException("Mic-E types cannot be decoded from just the info field. Use the specific class for decoding Mic-E");
            }

            switch (type)
            {
                case PacketType.PositionWithoutTimestampNoMessaging:
                case PacketType.PositionWithoutTimestampWithMessaging:
                case PacketType.PositionWithTimestampNoMessaging:
                case PacketType.PositionWithTimestampWithMessaging:
                    PositionInfo positionInfo = new PositionInfo(encodedInfoField);
                    return positionInfo.Position.IsWeatherSymbol() ? new WeatherInfo(positionInfo) : positionInfo;

                case PacketType.Status:
                    return new StatusInfo(encodedInfoField);

                case PacketType.MaidenheadGridLocatorBeacon:
                    return new MaidenheadBeaconInfo(encodedInfoField);

                case PacketType.Message:
                    return new MessageInfo(encodedInfoField);

                default:
                    return new UnsupportedInfo(encodedInfoField);
            }
        }

        /// <summary>
        /// Gets the <see cref="PacketType"/> of a string representation of an APRS info field.
        /// </summary>
        /// <param name="encodedInfoField">A string-encoded APRS info field.</param>
        /// <returns><see cref="PacketType"/> of the info field.</returns>
        public static PacketType GetPacketType(string encodedInfoField)
            => GetPacketType(Encoding.ASCII.GetBytes(encodedInfoField));

        /// <summary>
        /// Gets the <see cref="PacketType"/> of a string representation of an APRS info field.
        /// </summary>
        /// <param name="encodedInfoField">A string-encoded APRS info field.</param>
        /// <returns><see cref="PacketType"/> of the info field.</returns>
        public static PacketType GetPacketType(byte[] encodedInfoField)
        {
            if (encodedInfoField == null)
            {
                throw new ArgumentNullException(nameof(encodedInfoField));
            }

            // TODO Issue #67: This isn't always true.
            // '!' can come up to the 40th position.
            char dataTypeIdentifier = char.ToUpperInvariant((char)encodedInfoField[0]);

            return dataTypeIdentifier.ToPacketType();
        }

        /// <summary>
        /// Encodes an APRS info field to a string.
        /// </summary>
        /// <returns>String representation of the packet.</returns>
        public abstract string Encode();
    }
}
