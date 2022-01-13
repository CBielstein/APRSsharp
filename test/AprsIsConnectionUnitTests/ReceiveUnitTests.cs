namespace AprsSharpUnitTests.Connections.AprsIs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AprsSharp.Connections.AprsIs;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="AprsIsConnection.Receive(string, string, string, string?)"/>.
    /// </summary>
    public class ReceiveUnitTests
    {
        /// <summary>
        /// <see cref="AprsIsConnection.Receive(string, string, string, string?)"/> raises event on TCP message received.
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
            var aprsIs = new AprsIsConnection(mockTcpConnection.Object);
            aprsIs.ReceivedTcpMessage += (string message) =>
            {
                tcpMessagesReceived.Add(message);
                eventHandled = true;
            };

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", null);

            // Wait to ensure the message is received
            WaitForCondition(() => eventHandled, 500);

            // Assert the callback was triggered and that the expected message was received.
            Assert.True(eventHandled);
            Assert.Equal(1, tcpMessagesReceived.Count);
            Assert.Contains(testMessage, tcpMessagesReceived);
        }

        /// <summary>
        /// Tests that <see cref="AprsIsConnection.Receive(string, string, string, string?)"/> handles server login.
        /// </summary>
        [Fact]
        public void ReceiveHandlesLogin()
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
            var aprsIs = new AprsIsConnection(mockTcpConnection.Object);
            aprsIs.ReceivedTcpMessage += (string message) =>
            {
                tcpMessagesReceived.Add(message);
                eventHandled = true;
            };

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", "r/50.5039/4.4699/50");

            // Wait to ensure the messages are sent and received
            WaitForCondition(() => aprsIs.LoggedIn, 1500);

            // Assert the callback was triggered and that the expected message was received.
            Assert.True(eventHandled);
            Assert.Equal(2, tcpMessagesReceived.Count);
            Assert.Contains(firstMessage, tcpMessagesReceived);
            Assert.Contains(loginResponse, tcpMessagesReceived);

            // Assert that the login was completed
            Assert.True(aprsIs.LoggedIn);

            // Assert that a connection was started and the login message was sent to the server
            mockTcpConnection.Verify(mock => mock.Connect(
                    It.Is<string>(s => s.Equals("example.com", StringComparison.Ordinal)),
                    It.Is<int>(p => p == 14580)));

            mockTcpConnection.Verify(mock => mock.SendString(
                    It.Is<string>(m => m.Equals(expectedLoginMessage, StringComparison.Ordinal))));
        }

        /// <summary>
        /// Waits for a specified condition or thows an exception if a time limit is reached.
        /// </summary>
        /// <param name="condition">A function returnig bool to check.</param>
        /// <param name="timeoutMs">Timeout in milliseconds.</param>
        /// <returns>True, if the condition is met before the timeout. Otherwise, throws an exception.</returns>
        private static bool WaitForCondition(Func<bool> condition, int timeoutMs)
        {
            DateTime start = DateTime.UtcNow;

            while ((DateTime.UtcNow - start).TotalMilliseconds <= timeoutMs)
            {
                if (condition())
                {
                    return true;
                }

                Thread.Yield();
            }

            throw new TimeoutException();
        }
    }
}
