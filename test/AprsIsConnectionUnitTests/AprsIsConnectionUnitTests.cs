namespace AprsSharpUnitTests.Connections.AprsIs
{
    using System.Threading;
    using System.Threading.Tasks;
    using AprsSharp.Connections.AprsIs;
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
        public void TestDisconnect()
        {
            Mock<ITcpConnection> mockTcpConnection = new Mock<ITcpConnection>();
            AprsIsConnection connection = new AprsIsConnection(mockTcpConnection.Object);

            Task receiveTask = connection.Receive(null, null, null, null);

            // Sleep 0.01 seconds to ensure the connection starts "receiving"
            Thread.Sleep(10);

            connection.Disconnect();

            // Wait for the task, but no more than 2 seconds.
            Assert.True(Task.WaitAll(new Task[] { receiveTask }, 2000), "Task timed out while waiting for disconnection.");

            Assert.True(receiveTask.IsCompletedSuccessfully, "Task did not complete successfully.");
            mockTcpConnection.Verify(mock => mock.Connect(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockTcpConnection.Verify(mock => mock.ReceiveString(), Times.AtLeastOnce());
            mockTcpConnection.Verify(mock => mock.Disconnect(), Times.Once());
        }
    }
}
