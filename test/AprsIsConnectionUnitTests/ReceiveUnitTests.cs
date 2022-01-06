namespace AprsSharpUnitTests.Connections.AprsIs
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
        public void ReceivedTcpMessageEvent()
        {
            IList<string> tcpMessagesReceived = new List<string>();
            bool eventHandled = false;

            // Mock underlying TCP connection
            string testMessage = "This is a test message";
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.Setup(mock => mock.ReceiveString()).Returns(testMessage);

            // Create connection and register a callback
            var arpsIs = new AprsIsConnection(mockTcpConnection.Object);
            arpsIs.ReceivedTcpMessage += (string message) =>
            {
                tcpMessagesReceived.Add(message);
                eventHandled = true;
            };

            // Receive some packets from it.
            _ = arpsIs.Receive(null, null);

            // Wait to ensure the message is received
            while (!eventHandled)
            {
                Thread.Sleep(100);
            }

            // Assert the callback was triggered and that the expected message was received.
            Assert.True(eventHandled);
            Assert.Equal(1, tcpMessagesReceived.Count);
            Assert.Contains(testMessage, tcpMessagesReceived);
        }

        /// <summary>
        /// Tests that <see cref="AprsIsConnection.Receive(string?, string?)"/> handles server login.
        /// </summary>
        [Fact]
        public void ReceiveHandlesAuth()
        {
            IList<string> tcpMessagesReceived = new List<string>();
            bool eventHandled = false;
            string expectedLoginMessage = $"user N0CALL pass -1 vers AprsSharp 0.1 filter r/50.5039/4.4699/50";

            // Mock underlying TCP connection
            string firstMessage = "# server first message";
            string loginResponse = "# logresp N0CALL unverified, server TEST";
            var mockTcpConnection = new Mock<ITcpConnection>();

            mockTcpConnection.SetupSequence(mock => mock.ReceiveString())
                .Returns(firstMessage)
                .Returns(loginResponse);

            // Create connection and register a callback
            var arpsIs = new AprsIsConnection(mockTcpConnection.Object);
            arpsIs.ReceivedTcpMessage += (string message) =>
            {
                tcpMessagesReceived.Add(message);
                eventHandled = true;
            };

            // Receive some packets from it.
            _ = arpsIs.Receive("N0CALL", "-1");

            // Wait to ensure the messages are sent and received
            while (!eventHandled || tcpMessagesReceived.Count < 2)
            {
                Thread.Sleep(100);
            }

            // Assert the callback was triggered and that the expected message was received.
            Assert.True(eventHandled);
            Assert.Equal(2, tcpMessagesReceived.Count);
            Assert.Contains(firstMessage, tcpMessagesReceived);
            Assert.Contains(loginResponse, tcpMessagesReceived);

            // Assert that the login was completed
            Assert.True(arpsIs.LoggedIn);

            // Assert that the login message was sent to the server
            mockTcpConnection.Verify(mock =>
                mock.SendString(It.Is((string m) =>
                    m.Equals(expectedLoginMessage, System.StringComparison.InvariantCulture))));
        }
    }
}
