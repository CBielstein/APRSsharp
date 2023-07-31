namespace AprsSharpUnitTests.AprsIsClient
{
    using System.Threading.Tasks;
    using AprsSharp.AprsIsClient;
    using AprsSharp.Shared;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    /// <summary>
    /// Tests <see cref="AprsIsClient"/>.
    /// </summary>
    public class AprsIsClientUnitTests
    {
        /// <summary>
        /// Tests that the Disconnect() method on <see cref="AprsIsClient"/>
        /// succesfully stops receiving and the Receive() Task returns.
        /// </summary>
        [Fact]
        public async void TestDisconnect()
        {
            Mock<ITcpConnection> mockTcpConnection = new Mock<ITcpConnection>();
            mockTcpConnection.SetupGet(mock => mock.Connected).Returns(true);
            using AprsIsClient client = new AprsIsClient(NullLogger<AprsIsClient>.Instance, mockTcpConnection.Object);

            Task receiveTask = client.Receive("callsign", "password", "server", "filter");

            // Sleep 0.01 seconds to ensure the connection starts "receiving"
            await Task.Delay(10);

            client.Disconnect();

            // TODO Issue 125: Add a timeout for this test, if disconnect fails we could be here a while...
            await receiveTask;

            Assert.True(receiveTask.IsCompletedSuccessfully, "Task did not complete successfully.");
            mockTcpConnection.Verify(mock => mock.Connect(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockTcpConnection.Verify(mock => mock.ReceiveString(), Times.AtLeastOnce());
            mockTcpConnection.Verify(mock => mock.Disconnect(), Times.Once());
        }
    }
}
