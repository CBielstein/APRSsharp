namespace AprsSharpUnitTests.Connections.AprsIs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AprsSharp.Connections.AprsIs;
    using AprsSharp.Parsers.Aprs;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="AprsIsConnection.Receive(string, string, string, string?)"/>.
    /// </summary>
    public class ReceiveUnitTests
    {
        /// <summary>
        /// Verifies that the <see cref="AprsIsConnection.ReceivedTcpMessage"/> event is raised when
        /// a TCP message is received in <see cref="AprsIsConnection.Receive(string, string, string, string?)"/>.
        /// </summary>
        [Fact]
        public void ReceivedTcpMessageEvent()
        {
            IList<string> tcpMessagesReceived = new List<string>();
            bool eventHandled = false;

            // Mock underlying TCP connection
            string testMessage = "This is a test message";
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.SetupSequence(mock => mock.ReceiveString()).Returns(testMessage).Returns(string.Empty);

            // Create connection and register a callback
            var aprsIs = new AprsIsConnection(mockTcpConnection.Object, NullLogger<AprsIsConnection>.Instance);
            aprsIs.ReceivedTcpMessage += (string message) =>
            {
                tcpMessagesReceived.Add(message);
                eventHandled = true;
            };

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", null);

            // Wait to ensure the message is received
            WaitForCondition(() => eventHandled, 750);

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
            bool tcpMessageEventHandled = false;
            bool stateChangeEventHandled = false;
            IList<ConnectionState> stateChangesReceived = new List<ConnectionState>();

            string expectedLoginMessage = $"user N0CALL pass -1 vers AprsSharp 0.1.1 filter r/50.5039/4.4699/50";

            // Mock underlying TCP connection
            string firstMessage = "# server first message";
            string loginResponse = "# logresp N0CALL unverified, server TEST";
            var mockTcpConnection = new Mock<ITcpConnection>();

            mockTcpConnection.SetupSequence(mock => mock.ReceiveString())
                .Returns(firstMessage)
                .Returns(loginResponse);

            // Create connection and register callbacks
            var aprsIs = new AprsIsConnection(mockTcpConnection.Object, NullLogger<AprsIsConnection>.Instance);
            aprsIs.ReceivedTcpMessage += (string message) =>
            {
                tcpMessagesReceived.Add(message);
                tcpMessageEventHandled = true;
            };
            aprsIs.ChangedState += (ConnectionState newState) =>
            {
                stateChangesReceived.Add(newState);
                stateChangeEventHandled = true;
            };

            Assert.Equal(ConnectionState.NotConnected, aprsIs.State);

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", "r/50.5039/4.4699/50");

            // Wait to ensure the messages are sent and received
            WaitForCondition(() => aprsIs.State == ConnectionState.LoggedIn, 1500);

            // Assert the state change event was triggered with the correct state
            Assert.True(stateChangeEventHandled);
            Assert.Equal(2, stateChangesReceived.Count);
            Assert.Equal(ConnectionState.Connected, stateChangesReceived[0]);
            Assert.Equal(ConnectionState.LoggedIn, stateChangesReceived[1]);

            // Assert the TCP message callbacks were triggered with the correct messages
            Assert.True(tcpMessageEventHandled);
            Assert.Equal(2, tcpMessagesReceived.Count);
            Assert.Equal(firstMessage, tcpMessagesReceived[0]);
            Assert.Equal(loginResponse, tcpMessagesReceived[1]);

            // Assert that the login was completed
            Assert.Equal(ConnectionState.LoggedIn, aprsIs.State);

            // Assert that a connection was started and the login message was sent to the server
            mockTcpConnection.Verify(mock => mock.Connect(
                    It.Is<string>(s => s.Equals("example.com", StringComparison.Ordinal)),
                    It.Is<int>(p => p == 14580)));

            mockTcpConnection.Verify(mock => mock.SendString(
                    It.Is<string>(m => m.Equals(expectedLoginMessage, StringComparison.Ordinal))));
        }

        /// <summary>
        /// Verifies that the <see cref="AprsIsConnection.ReceivedPacket"/> event is raised when
        /// a packet is decoded in <see cref="AprsIsConnection.Receive(string, string, string, string?)"/>.
        /// </summary>
        [Fact]
        public void ReceivedPacketEvent()
        {
            IList<string> tcpMessagesReceived = new List<string>();
            bool eventHandled = false;
            Packet? receivedPacket = null;

            // Mock underlying TCP connection
            string encodedPacket = @"N0CALL>igate,T2serv:>CN76wv\L Lighthouse!";
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.Setup(mock => mock.ReceiveString()).Returns(encodedPacket);

            // Create connection and register a callback
            var aprsIs = new AprsIsConnection(mockTcpConnection.Object, NullLogger<AprsIsConnection>.Instance);
            aprsIs.ReceivedPacket += (Packet p) =>
            {
                receivedPacket = p;
                eventHandled = true;
            };

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", null);

            // Wait to ensure the message is received
            WaitForCondition(() => eventHandled, 1250);

            // Assert the callback was triggered and that the expected message was received.
            Assert.True(eventHandled);

            if (receivedPacket == null)
            {
                Assert.NotNull(receivedPacket);
            }
            else
            {
                Assert.Equal(PacketType.Status, receivedPacket.InfoField.Type);
                Assert.IsType<StatusInfo>(receivedPacket.InfoField);

                StatusInfo si = (StatusInfo)receivedPacket.InfoField;

                Assert.NotNull(si.Position);
                Assert.Equal("CN76wv", si.Position?.EncodeGridsquare(6, false), ignoreCase: true);
                Assert.Equal('\\', si.Position?.SymbolTableIdentifier);
                Assert.Equal('L', si.Position?.SymbolCode);
                Assert.Null(si.Timestamp);
                Assert.Equal("Lighthouse!", si.Comment);
            }
        }

        /// <summary>
        /// Validates that an error while connecting to the TCP server
        /// results in a disconnected event sent by the <see cref="AprsIsConnection"/>.
        /// </summary>
        [Fact]
        public void FailureToConnectSetsDisconnectedState()
        {
            IList<ConnectionState> stateChangesReceived = new List<ConnectionState>();

            // Mock underlying TCP connection
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.SetupSequence(
                mock => mock.Connect(It.IsAny<string>(), It.IsAny<int>()))
                    .Throws(new Exception("Mock exception connecting!"));

            // Create connection and register callback
            var aprsIs = new AprsIsConnection(mockTcpConnection.Object, NullLogger<AprsIsConnection>.Instance);
            aprsIs.ChangedState += (ConnectionState newState) => stateChangesReceived.Add(newState);
            Assert.Equal(ConnectionState.NotConnected, aprsIs.State);

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", "r/50.5039/4.4699/50");

            // Wait to ensure the messages are sent and received
            WaitForCondition(() => aprsIs.State == ConnectionState.Disconnected, 1500);

            // Assert the state change event was triggered with the correct state
            Assert.Equal(1, stateChangesReceived.Count);
            Assert.Equal(ConnectionState.Disconnected, stateChangesReceived[0]);
            Assert.Equal(ConnectionState.Disconnected, aprsIs.State);

            // Assert that a connection was started and the login message was sent to the server
            mockTcpConnection.Verify(mock => mock.Connect(
                    It.Is<string>(s => s.Equals("example.com", StringComparison.Ordinal)),
                    It.Is<int>(p => p == 14580)));
        }

        /// <summary>
        /// Tests that full cycle of connection, login, disconnected states on
        /// <see cref="AprsIsConnection"/> and that the proper events are raised for them.
        /// </summary>
        [Fact]
        public void FailureToReceiveSetsDisconnectedState()
        {
            IList<ConnectionState> stateChangesReceived = new List<ConnectionState>();

            string expectedLoginMessage = $"user N0CALL pass -1 vers AprsSharp 0.1 filter r/50.5039/4.4699/50";

            // Mock underlying TCP connection
            string firstMessage = "# server first message";
            string loginResponse = "# logresp N0CALL unverified, server TEST";
            var mockTcpConnection = new Mock<ITcpConnection>();

            mockTcpConnection.SetupSequence(mock => mock.ReceiveString())
                .Returns(firstMessage)
                .Returns(loginResponse)
                .Throws(new Exception("Something happened to the connection!"));

            // Create connection and register callbacks
            var aprsIs = new AprsIsConnection(mockTcpConnection.Object, NullLogger<AprsIsConnection>.Instance);
            aprsIs.ChangedState += (ConnectionState newState) => stateChangesReceived.Add(newState);

            // Start receiving
            Assert.Equal(ConnectionState.NotConnected, aprsIs.State);
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", "r/50.5039/4.4699/50");

            // Wait to ensure the messages are sent and received
            WaitForCondition(() => aprsIs.State == ConnectionState.Disconnected, 1500);

            // Assert the state change event was triggered with the correct state

            Assert.False(true, "Add assertions around logging. I would expect a log error event to be tossed up.");
            Assert.Equal(3, stateChangesReceived.Count);
            Assert.Equal(ConnectionState.Connected, stateChangesReceived[0]);
            Assert.Equal(ConnectionState.LoggedIn, stateChangesReceived[1]);
            Assert.Equal(ConnectionState.Disconnected, stateChangesReceived[2]);
            Assert.Equal(ConnectionState.Disconnected, aprsIs.State);
        }

        /// <summary>
        /// Ensures that packets failing decode are successfully logged.
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel"/> for this test to use.</param>
        /// <param name="shouldLog">If an event should be logged given the loglevel set.</param>
        [Theory]
        [InlineData(LogLevel.Information, true)]
        [InlineData(LogLevel.Warning, false)]
        public void LogsFailingDecode(LogLevel logLevel, bool shouldLog)
        {
            throw new NotImplementedException();
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
