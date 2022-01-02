namespace AprsSharpUnitTests.Parsers.Aprs
{
    using AprsSharp.Parsers.Aprs;
    using AprsSharp.Parsers.Aprs.Extensions;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="EnumConversionExtensionsUnitTests"/> class.
    /// </summary>
    public class EnumConversionExtensionsUnitTests
    {
        /// <summary>
        /// Tests GetTypeChar.
        /// </summary>
        /// <param name="packetType"><see cref="PacketType"/> to test.</param>
        /// <param name="expectedChar">Expected character result.</param>
        [Theory]
        [InlineData(PacketType.PositionWithoutTimestampWithMessaging, '=')]
        public void PacketTypeConversion(
            PacketType packetType,
            char expectedChar)
        {
            Assert.Equal(expectedChar, packetType.ToChar());
            Assert.Equal(packetType, expectedChar.ToPacketType());
        }
    }
}
