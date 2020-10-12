namespace AprsIsUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AprsISLibrary;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for APRS IS library.
    /// </summary>
    public class UnitTest1
    {
        private readonly IList<string> tcpMessagesReceived = new List<string>();

        /// <summary>
        /// Receive raises event on TCP message received.
        /// </summary>
        [Fact]
        public void TestReceiveEvent()
        {
            // Setup
            // Create an APRS IS connection object.
            var mockTcpConnection = new Mock<ITcpConnection>();
            string testMessage = "This is a test message";
            mockTcpConnection.Setup(mock => mock.ReceiveString()).Returns(testMessage);

            var arpsIs = new AprsISLib(mockTcpConnection.Object);
            arpsIs.ReceivedTcpMessage += TestTcpHandler;

            // Action
            // Receive some packets from it.
            _ = arpsIs.Receive();

            Thread.Sleep(1000);

            // Assertions
            Assert.NotEmpty(tcpMessagesReceived);
            Assert.Contains(testMessage, tcpMessagesReceived);
        }

        private void TestTcpHandler(string tcpMessage)
        {
            tcpMessagesReceived.Add(tcpMessage);
        }
    }
}
