namespace AprsSharpUnitTests.Connections.AprsIs
{
    using System.Threading.Tasks;
    using AprsSharp.Connections.AprsIs;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    /// <summary>
    /// Tests AprsIsConnection.
    /// </summary>
    public class AprsIsConnectionUnitTests
    {
        /// <summary>
        /// Tests that the Disconnect() method on AprsIsConnection
        /// succesfully stops receiving and the Receive() Task returns.
        /// </summary>
        [Fact]
        public async void TestDisconnect()
        {
            Mock<ITcpConnection> mockTcpConnection = new Mock<ITcpConnection>();
            using AprsIsConnection connection = new AprsIsConnection(NullLogger<AprsIsConnection>.Instance, mockTcpConnection.Object);

            Task receiveTask = connection.Receive("callsign", "password", "server", "filter");

            // Sleep 0.01 seconds to ensure the connection starts "receiving"
            await Task.Delay(10);

            connection.Disconnect();

            // TODO Issue 125: Add a timeout for this test, if disconnect fails we could be here a while...
            await receiveTask;

            Assert.True(receiveTask.IsCompletedSuccessfully, "Task did not complete successfully.");
            mockTcpConnection.Verify(mock => mock.Connect(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockTcpConnection.Verify(mock => mock.ReceiveString(), Times.AtLeastOnce());
            mockTcpConnection.Verify(mock => mock.Disconnect(), Times.Once());
        }
    }
}
