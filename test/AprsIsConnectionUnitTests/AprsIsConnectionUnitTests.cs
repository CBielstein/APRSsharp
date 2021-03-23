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
        /// Tests that the `Disconnect()` method on AprsIsConnection
        /// succesfully stops receiving.
        /// </summary>
        [Fact]
        public void TestDisconnect()
        {
            Mock<ITcpConnection> mockTcpConnection = new Mock<ITcpConnection>();
            AprsIsConnection connection = new AprsIsConnection(mockTcpConnection.Object);

            Task receiveTask = connection.Receive(null, null);
            connection.Disconnect();

            Thread.Sleep(100); // Sleep 0.1 seconds to allow connection to cancel

            Assert.True(receiveTask.IsCompletedSuccessfully);
            mockTcpConnection.Verify(mock => mock.Disconnect(), Times.Once());
        }
    }
}
