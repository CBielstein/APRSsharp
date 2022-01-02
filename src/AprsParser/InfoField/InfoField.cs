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

    /*
        /// <summary>
        /// Decodes a position from raw GPS location (NMEA formats)
        /// This expects a three letter type to start the string
        /// Supported formats:
        ///     None yet.
        /// </summary>
        /// <param name="rawGpsPacket">The full packet. Decoded.</param>
        public static void HandleRawGps(string rawGpsPacket)
        {
            if (rawGpsPacket == null)
            {
                throw new ArgumentNullException(nameof(rawGpsPacket));
            }
            else if (rawGpsPacket.Length < 6)
            {
                // 6 is the length of the identifier $GPxxx, so the string is invalid if it isn't at least that long
                throw new ArgumentException("rawGpsPacket should be 6 or more characters in length. Given length: " + rawGpsPacket.Length);
            }

            // Ensure start of identifier is valid
            if (!rawGpsPacket.Substring(0, 3).Equals("$GP", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentNullException("rawGpsPacket should have started with $GP. Given packet started with:" + rawGpsPacket.Substring(0, 3));
            }

            // Get type of packet
            NmeaType nmeaDataType = rawGpsPacket.Substring(3, 3).ToNmeaType();

            throw new NotImplementedException("handle RawGPSData");
        }

        /// <summary>
        /// Takes a string representing the AX.25 Information Field or (as it's called in this case) the APRS Information Field
        /// and populates this object with the information therein.
        /// </summary>
        /// <param name="informationField">string representation of the APRS Information Field.</param>
        public void DecodeInformationField(string informationField)
        {
            if (informationField == null)
            {
                throw new ArgumentNullException(nameof(informationField));
            }
            else if (informationField.Length == 0)
            {
                throw new ArgumentException("Empty string argument");
            }

            DecodedType = GetDataType(informationField);

            switch (DecodedType)
            {
                case PacketType.RawGPSData:
                    HandleRawGps(informationField);
                    break;

                case PacketType.WeatherReport: // TODO raw weather reports vs positionless?
                    // handle Weather report(without position)
                    Timestamp = new Timestamp(informationField.Substring(1, 8));
                    throw new NotImplementedException("handle Weather report (without position)");
            }
        }
    }*/
}
