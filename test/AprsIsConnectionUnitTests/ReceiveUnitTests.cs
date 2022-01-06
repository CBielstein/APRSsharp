namespace AprsIsUnitTests
{
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
        /// <summary>
        /// Receive raises event on TCP message received.
        /// </summary>
        [Fact]
        public void TestReceivedTcpMessageEvent()
        {
            IList<string> tcpMessagesReceived = new List<string>();
            bool eventHandled = false;
            string testMessage = "This is a test message";

            // Create an APRS IS connection object.
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.Setup(mock => mock.ReceiveString()).Returns(testMessage);

            var arpsIs = new AprsIsConnection(mockTcpConnection.Object);
            arpsIs.ReceivedTcpMessage += (string message) =>
            {
                tcpMessagesReceived.Add(message);
                eventHandled = true;
            };

            // Receive some packets from it.
            _ = arpsIs.Receive(null, null);

            // Wait to ensure the message is sent
            while (!eventHandled)
            {
                Thread.Sleep(100);
            }

            Assert.Equal(1, tcpMessagesReceived.Count);
            Assert.NotEmpty(tcpMessagesReceived);
            Assert.Contains(testMessage, tcpMessagesReceived);
        }
    }
}
