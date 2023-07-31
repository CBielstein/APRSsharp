namespace AprsSharpUnitTests.AprsParser
{
    using AprsSharp.AprsParser;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="InfoField"/> class related to encode/decode of the information field.
    /// </summary>
    public class InfoFieldUnitTests
    {
        /// <summary>
        /// Tests GetDataType.
        /// </summary>
        /// <param name="informationField">Input information field to test.</param>
        /// <param name="expectedDataType">Expected data type result.</param>
        [Theory]
        [InlineData("/092345z4903.50N/07201.75W>Test1234", PacketType.PositionWithTimestampNoMessaging)]
        [InlineData(">IO91SX/G", PacketType.Status)]
        public void GetDataType(
            string informationField,
            PacketType expectedDataType)
        {
            InfoField infoField = InfoField.FromString(informationField);
            Assert.Equal(expectedDataType, infoField.Type);
        }
    }
}
