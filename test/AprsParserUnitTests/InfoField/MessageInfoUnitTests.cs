namespace AprsSharpUnitTests.Parsers.Aprs
{
    using System;
    using System.Linq;
    using AprsSharp.Parsers.Aprs;
    using GeoCoordinatePortable;
    using Xunit;

    /// <summary>
    /// Tests code in the <see cref="MessageInfo"/> class related to encode/decode of position reports.
    /// </summary>
    public class MessageInfoUnitTests
    {
        /// <summary>
        /// Verifies decoding and re-encoding a full message packet.
        /// </summary>
        [Fact]
        public void TestRoundTrip()
        {
            string encoded = "N0CALL>WIDE2-2::Test     :Test Message{123ab";
            Packet p = new Packet(encoded);

            Assert.Equal("N0CALL", p.Sender);
            Assert.Equal("WIDE2-2", p.Path.Single());
            Assert.True((p.ReceivedTime - DateTime.UtcNow) < TimeSpan.FromMinutes(1));
            Assert.Equal(PacketType.Message, p.InfoField.Type);

            if (p.InfoField is MessageInfo mi)
            {
                Assert.Equal("Test", mi.Addressee);
                Assert.Equal("Test Message", mi.Content);
                Assert.Equal("123ab", mi.Id);
            }
            else
            {
                Assert.IsType<MessageInfo>(p.InfoField);
            }

            Assert.Equal(encoded, p.Encode());
        }

        /// <summary>
        /// Tests decoding a message info field based on the APRS spec.
        /// </summary>
        /// <param name="informationField">Information field for decode.</param>
        /// <param name="expectedAddressee">Expected decoded addressee.</param>
        /// <param name="expectedContent">Expected decoded content.</param>
        /// <param name="expectedId">Expected decoded message ID.</param>
        [Theory]
        [InlineData(":WU2Z     :Testing", "WU2Z", "Testing", null)] // From APRS 1.01
        [InlineData(":WU2Z     :Testing{003", "WU2Z", "Testing", "003")] // From APRS 1.01
        [InlineData(":EMAIL    :msproul@ap.org Test email", "EMAIL", "msproul@ap.org Test email", null)] // From APRS 1.01
        [InlineData(":Testing12:{3", "Testing12", null, "3")] // No content
        public void DecodeMessageFormat(
            string informationField,
            string expectedAddressee,
            string expectedContent,
            string? expectedId)
        {
            MessageInfo mi = new MessageInfo(informationField);

            Assert.Equal(expectedAddressee, mi.Addressee);
            Assert.Equal(expectedContent, mi.Content);
            Assert.Equal(expectedId, mi.Id);
        }

        /// <summary>
        /// Tests encoding a message info field based on the APRS spec.
        /// </summary>
        /// <param name="addressee">Expected decoded addressee.</param>
        /// <param name="content">Expected decoded content.</param>
        /// <param name="id">Expected decoded message ID.</param>
        /// <param name="expectedEncoding">Information field for decode.</param>
        [Theory]
        [InlineData("WU2Z", "Testing", null, ":WU2Z     :Testing")] // From APRS 1.01
        [InlineData("WU2Z", "Testing", "003", ":WU2Z     :Testing{003")] // From APRS 1.01
        [InlineData("EMAIL", "msproul@ap.org Test email", null, ":EMAIL    :msproul@ap.org Test email")] // From APRS 1.01
        [InlineData("Testing12", null, "3", ":Testing12:{3")] // No content
        public void EncodeMessageFormat(
            string addressee,
            string? content,
            string? id,
            string expectedEncoding)
        {
            MessageInfo mi = new MessageInfo(addressee, content, id);
            Assert.Equal(expectedEncoding, mi.Encode());
        }

        /// <summary>
        /// Validates that message ID must be alphanumeric.
        /// </summary>
        [Fact]
        public void NonAlphanumericIdRejected()
        {
            var exception = Assert.Throws<ArgumentException>(() => new MessageInfo("test", "test", "!@#$%"));
            Assert.Equal("If provided, ID must be only alphanumeric (Parameter 'messageId')", exception.Message);
        }

        /// <summary>
        /// Validates that disallowed characters in content result in exception.
        /// </summary>
        /// <param name="content">Content to test.</param>
        [Theory]
        [InlineData("Test{")]
        [InlineData("Test~")]
        [InlineData("Test|")]
        public void DisallowedContentCharactersRejected(string content)
        {
            var exception = Assert.Throws<ArgumentException>(() => new MessageInfo("test", content, null));
            Assert.Equal("Message content may not include `|`, `~`, or `{` (Parameter 'content')", exception.Message);
        }
    }
}
