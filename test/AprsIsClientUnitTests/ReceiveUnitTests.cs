namespace AprsSharpUnitTests.AprsIsClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AprsSharp.AprsIsClient;
    using AprsSharp.AprsParser;
    using AprsSharp.Shared;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="AprsIsClient.Receive(string, string, string, string?)"/>.
    /// </summary>
    [Collection(nameof(TimedTests))]
    public class ReceiveUnitTests
    {
        /// <summary>
        /// Verifies that the <see cref="AprsIsClient.ReceivedTcpMessage"/> event is raised when
        /// a TCP message is received in <see cref="AprsIsClient.Receive(string, string, string, string?)"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact(Timeout = 500)]
        public async Task ReceivedTcpMessageEvent()
        {
            List<string> tcpMessagesReceived = new List<string>();
            TaskCompletionSource eventHandled = new TaskCompletionSource();

            // Mock underlying TCP connection
            string testMessage = "This is a test message";
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.SetupSequence(mock => mock.ReceiveString()).Returns(testMessage).Returns(string.Empty);
            mockTcpConnection.SetupGet(mock => mock.Connected).Returns(true);

            // Create connection and register a callback
            using var aprsIs = new AprsIsClient(mockTcpConnection.Object);
            aprsIs.ReceivedTcpMessage += (string message) =>
            {
                tcpMessagesReceived.Add(message);
                eventHandled.SetResult();
            };

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", null);

            // Wait to ensure the message is received
            await eventHandled.Task;

            // Assert the callback was triggered and that the expected message was received.
            Assert.Single(tcpMessagesReceived);
            Assert.Contains(testMessage, tcpMessagesReceived);
        }

        /// <summary>
        /// Tests that <see cref="AprsIsClient.Receive(string, string, string, string?)"/> handles server login.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact(Timeout = 500)]
        public async Task ReceiveHandlesLogin()
        {
            List<string> tcpMessagesReceived = new List<string>();
            List<ConnectionState> stateChangesReceived = new List<ConnectionState>();
            TaskCompletionSource loggedIn = new TaskCompletionSource();

            string expectedLoginMessage = $"user N0CALL pass -1 vers AprsSharp 0.4.0 filter r/50.5039/4.4699/50";

            // Mock underlying TCP connection
            string firstMessage = "# server first message";
            string loginResponse = "# logresp N0CALL unverified, server TEST";
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.SetupGet(m => m.Connected).Returns(true);

            mockTcpConnection.SetupSequence(mock => mock.ReceiveString())
                .Returns(firstMessage)
                .Returns(loginResponse);

            // Create connection and register callbacks
            using var aprsIs = new AprsIsClient(mockTcpConnection.Object);
            aprsIs.ReceivedTcpMessage += (string message) =>
            {
                tcpMessagesReceived.Add(message);
            };
            aprsIs.ChangedState += (ConnectionState newState) =>
            {
                stateChangesReceived.Add(newState);

                if (newState == ConnectionState.LoggedIn)
                {
                    loggedIn.SetResult();
                }
            };

            Assert.Equal(ConnectionState.NotConnected, aprsIs.State);

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", "r/50.5039/4.4699/50");

            // Wait to ensure the messages are sent and received
            await loggedIn.Task;

            // Assert the state change event was triggered with the correct state
            Assert.Equal(2, stateChangesReceived.Count);
            Assert.Equal(ConnectionState.Connected, stateChangesReceived[0]);
            Assert.Equal(ConnectionState.LoggedIn, stateChangesReceived[1]);

            // Assert the TCP message callbacks were triggered with the correct messages
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
        /// Tests that <see cref="AprsIsClient.Receive(string, string, string, string?)"/> returns the correct server name in the ConnectedServer property.
        /// </summary>
        /// <param name="loginResponse">The test login response string returned from the APRS server.</param>
        /// <param name="expected">The server name expected to be set in ConnectedServer.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(Timeout = 500)]
        [InlineData("# logresp N0CALL unverified, server T2ONTARIO", "T2ONTARIO")]
        [InlineData("# logresp N0CALL verified, server T2ONTARIO", "T2ONTARIO")]
        [InlineData("# logresp N0CALL unverified, server T2BRAZIL", "T2BRAZIL")]
        [InlineData("# logresp N0CALL unverified, server T2BRAZIL serverCommand", "T2BRAZIL")]
        public async Task ReceiveSetConnectedServerProperty(string loginResponse, string expected)
        {
            // Mock underlying TCP connection
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.SetupGet(m => m.Connected).Returns(true);

            mockTcpConnection.SetupSequence(mock => mock.ReceiveString())
                .Returns(loginResponse);

            TaskCompletionSource loggedIn = new TaskCompletionSource();

            // Create connection and register callbacks
            using var aprsIs = new AprsIsClient(mockTcpConnection.Object);
            aprsIs.ChangedState += (ConnectionState newState) =>
            {
                if (newState == ConnectionState.LoggedIn)
                {
                    loggedIn.SetResult();
                }
            };

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", "r/50.5039/4.4699/50");

            // Wait to ensure the messages are sent and received
            await loggedIn.Task;

            // Assert the ConnectedServer property was set to the correct server name.
            Assert.Equal(expected, aprsIs.ConnectedServer);
        }

        /// <summary>
        /// Verifies that the <see cref="AprsIsClient.ReceivedPacket"/> event is raised when
        /// a packet is decoded in <see cref="AprsIsClient.Receive(string, string, string, string?)"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact(Timeout = 500)]
        public async Task ReceivedPacketEvent()
        {
            TaskCompletionSource eventHandled = new TaskCompletionSource();
            Packet? receivedPacket = null;

            // Mock underlying TCP connection
            string encodedPacket = @"N0CALL>igate,T2serv:>CN76wv\L Lighthouse!";
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.Setup(mock => mock.ReceiveString()).Returns(encodedPacket);
            mockTcpConnection.SetupGet(mock => mock.Connected).Returns(true);

            // Create connection and register a callback
            using var aprsIs = new AprsIsClient(mockTcpConnection.Object);
            aprsIs.ReceivedPacket += (Packet p) =>
            {
                receivedPacket = p;
                eventHandled.SetResult();
            };

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", null);

            // Wait to ensure the message is received
            await eventHandled.Task;

            // Assert the callback was triggered and that the expected message was received.
            Assert.NotNull(receivedPacket);
            Assert.Equal(PacketType.Status, receivedPacket.InfoField.Type);
            Assert.IsType<StatusInfo>(receivedPacket.InfoField);

            StatusInfo si = (StatusInfo)receivedPacket.InfoField;

            Assert.NotNull(si.Position);
            Assert.Equal("CN76wv", si.Position.EncodeGridsquare(6, false), ignoreCase: true);
            Assert.Equal('\\', si.Position.SymbolTableIdentifier);
            Assert.Equal('L', si.Position.SymbolCode);
            Assert.Null(si.Timestamp);
            Assert.Equal("Lighthouse!", si.Comment);
        }

        /// <summary>
        /// Validates that an error while connecting to the TCP server
        /// results in a disconnected event sent by the <see cref="AprsIsClient"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact(Timeout = 500)]
        public async Task FailureToConnectSetsDisconnectedState()
        {
            List<ConnectionState> stateChangesReceived = new List<ConnectionState>();
            TaskCompletionSource disconnected = new TaskCompletionSource();

            // Mock underlying TCP connection
            var mockTcpConnection = new Mock<ITcpConnection>();
#pragma warning disable CA2201 // Do not raise reserved exception types
            mockTcpConnection.SetupSequence(
                mock => mock.Connect(It.IsAny<string>(), It.IsAny<int>()))
                    .Throws(new Exception("Mock exception connecting!"));
#pragma warning restore CA2201 // Do not raise reserved exception types

            // Create connection and register callback
            using var aprsIs = new AprsIsClient(mockTcpConnection.Object);
            aprsIs.ChangedState += (ConnectionState newState) =>
            {
                stateChangesReceived.Add(newState);
                if (newState == ConnectionState.Disconnected)
                {
                    disconnected.SetResult();
                }
            };
            Assert.Equal(ConnectionState.NotConnected, aprsIs.State);

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", "r/50.5039/4.4699/50");

            // Wait to ensure the messages are sent and received
            await disconnected.Task;

            // Assert the state change event was triggered with the correct state
            Assert.Single(stateChangesReceived);
            Assert.Equal(ConnectionState.Disconnected, stateChangesReceived[0]);
            Assert.Equal(ConnectionState.Disconnected, aprsIs.State);

            // Assert that a connection was started and the login message was sent to the server
            mockTcpConnection.Verify(mock => mock.Connect(
                    It.Is<string>(s => s.Equals("example.com", StringComparison.Ordinal)),
                    It.Is<int>(p => p == 14580)));
        }

        /// <summary>
        /// Tests that full cycle of connection, login, disconnected states on
        /// <see cref="AprsIsClient"/> and that the proper events are raised for them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact(Timeout = 500)]
        public async Task FailureToReceiveSetsDisconnectedState()
        {
            List<ConnectionState> stateChangesReceived = new List<ConnectionState>();
            TaskCompletionSource disconnected = new TaskCompletionSource();

            string expectedLoginMessage = $"user N0CALL pass -1 vers AprsSharp 0.1 filter r/50.5039/4.4699/50";

            // Mock underlying TCP connection
            string firstMessage = "# server first message";
            string loginResponse = "# logresp N0CALL unverified, server TEST";
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.SetupGet(mock => mock.Connected).Returns(true);

#pragma warning disable CA2201 // Do not raise reserved exception types
            mockTcpConnection.SetupSequence(mock => mock.ReceiveString())
                .Returns(firstMessage)
                .Returns(loginResponse)
                .Throws(new Exception("Something happened to the connection!"));
#pragma warning restore CA2201 // Do not raise reserved exception types

            // Create connection and register callbacks
            using var aprsIs = new AprsIsClient(mockTcpConnection.Object);
            aprsIs.ChangedState += (ConnectionState newState) =>
            {
                stateChangesReceived.Add(newState);
                if (newState == ConnectionState.Disconnected)
                {
                    disconnected.SetResult();
                }
            };

            // Start receiving
            Assert.Equal(ConnectionState.NotConnected, aprsIs.State);
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", "r/50.5039/4.4699/50");

            // Wait to ensure the messages are sent and received
            await disconnected.Task;

            // Assert the state change event was triggered with the correct state
            Assert.Equal(3, stateChangesReceived.Count);
            Assert.Equal(ConnectionState.Connected, stateChangesReceived[0]);
            Assert.Equal(ConnectionState.LoggedIn, stateChangesReceived[1]);
            Assert.Equal(ConnectionState.Disconnected, stateChangesReceived[2]);
            Assert.Equal(ConnectionState.Disconnected, aprsIs.State);
        }

        /// <summary>
        /// Tests that a server disconnection will set a disconnected state.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact(Timeout = 500)]
        public async Task ServerDisconnectSetsDisconnectedState()
        {
            List<ConnectionState> stateChangesReceived = new List<ConnectionState>();
            TaskCompletionSource disconnected = new TaskCompletionSource();

            string expectedLoginMessage = $"user N0CALL pass -1 vers AprsSharp 0.1 filter r/50.5039/4.4699/50";

            // Mock underlying TCP connection
            string firstMessage = "# server first message";
            string loginResponse = "# logresp N0CALL unverified, server TEST";
            var mockTcpConnection = new Mock<ITcpConnection>();

            // Return a server message, login response, and packet
            // then start returning empty (as would happen on disconnect)
            // and set <see cref="ITcpConnection.Conncted"/> to false
            mockTcpConnection.SetupSequence(mock => mock.ReceiveString())
                .Returns(firstMessage)
                .Returns(loginResponse)
                .Returns(@"N0CALL>igate,T2serv:>CN76wv\L Lighthouse!")
                .Returns(string.Empty)
                .Returns(string.Empty);

            mockTcpConnection.SetupSequence(mock => mock.Connected)
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(false);

            // Create connection and register callbacks
            using var aprsIs = new AprsIsClient(mockTcpConnection.Object);
            aprsIs.ChangedState += (ConnectionState newState) =>
            {
                stateChangesReceived.Add(newState);

                if (newState == ConnectionState.Disconnected)
                {
                    disconnected.SetResult();
                }
            };

            // Start receiving
            Assert.Equal(ConnectionState.NotConnected, aprsIs.State);
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", "r/50.5039/4.4699/50");

            // Wait to ensure the messages are sent and received
            await disconnected.Task;

            // Assert the state change event was triggered with the correct state
            Assert.Equal(3, stateChangesReceived.Count);
            Assert.Equal(ConnectionState.Connected, stateChangesReceived[0]);
            Assert.Equal(ConnectionState.LoggedIn, stateChangesReceived[1]);
            Assert.Equal(ConnectionState.Disconnected, stateChangesReceived[2]);
            Assert.Equal(ConnectionState.Disconnected, aprsIs.State);

            // Assert we only checked connection and receive the correct number of times
            mockTcpConnection.VerifyGet(mock => mock.Connected, Times.Exactly(5));
            mockTcpConnection.Verify(mock => mock.ReceiveString(), Times.Exactly(4));
        }

        /// <summary>
        /// Tests that an event is raised on a failed decode.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact(Timeout = 500)]
        public async Task EventRaisedOnFailedDecode()
        {
            List<string> failedDecodes = new List<string>();
            List<Exception> reportedExceptions = new List<Exception>();
            TaskCompletionSource eventHandled = new TaskCompletionSource();

            // Mock underlying TCP connection
            string encodedPacket = @"BAD_PKT";
            var mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.SetupGet(mock => mock.Connected).Returns(true);

            string firstMessage = "# server first message";
            string loginResponse = "# logresp N0CALL unverified, server TEST";

            mockTcpConnection.SetupSequence(mock => mock.ReceiveString())
                .Returns(firstMessage)
                .Returns(loginResponse)
                .Returns(encodedPacket);

            // Create connection and register a callback
            using var aprsIs = new AprsIsClient(mockTcpConnection.Object);
            aprsIs.DecodeFailed += (Exception ex, string s) =>
            {
                reportedExceptions.Add(ex);
                failedDecodes.Add(s);
                eventHandled.SetResult();
            };

            // Receive some packets from it.
            _ = aprsIs.Receive("N0CALL", "-1", "example.com", null);

            // Wait to ensure the message is received
            await eventHandled.Task;

            // Assert the callback was triggered and that the expected message was received.
            Assert.Single(failedDecodes);
            Assert.Equal(encodedPacket, failedDecodes.Single());

            // Assert that the correct exception was raised
            Assert.Single(reportedExceptions);
            Assert.IsType<ArgumentOutOfRangeException>(reportedExceptions.Single());
        }
    }
}
