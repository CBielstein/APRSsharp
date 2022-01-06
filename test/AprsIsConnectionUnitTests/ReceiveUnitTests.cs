namespace AprsIsUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AprsSharp.Connections.AprsIs;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for the Receive method on <see cref="AprsIsConnection"/>.
    /// </summary>
    public class ReceiveUnitTests
    {
        private readonly IList<string> tcpMessagesReceived = new List<string>();
        private bool eventHandled = false;

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

            var arpsIs = new AprsIsConnection(mockTcpConnection.Object);
            arpsIs.ReceivedTcpMessage += TestTcpHandler;

            // Action
            // Receive some packets from it.
            _ = arpsIs.Receive(null, null);

            while (!eventHandled)
            {
                Thread.Sleep(100);
            }

            // Assertions
            Assert.NotEmpty(tcpMessagesReceived);
            Assert.Contains(testMessage, tcpMessagesReceived);
        }

        private void TestTcpHandler(string tcpMessage)
        {
            tcpMessagesReceived.Add(tcpMessage);
            eventHandled = true;
        }
    }
}
