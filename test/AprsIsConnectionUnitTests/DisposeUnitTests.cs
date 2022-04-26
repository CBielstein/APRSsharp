namespace AprsSharpUnitTests.Connections.AprsIs
{
    using AprsSharp.Connections.AprsIs;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="AprsIsConnection.Dispose()"/>.
    /// </summary>
    public class DisposeUnitTests
    {
        /// <summary>
        /// Ensures <see cref="AprsIsConnection.Dispose()"/> properly disposes
        /// the injected <see cref="ITcpConnection"/>.
        /// </summary>
        /// <param name="connectionShouldDispose">`true` if the mock <see cref="ITcpConnection"/> should be disposed by the
        ///     <see cref="AprsIsConnection"/> under test.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ProperlyHandlesInjectedTcpConnection(bool connectionShouldDispose)
        {
            var mockTcpConnection = new Mock<ITcpConnection>();

            using (AprsIsConnection connection = new AprsIsConnection(NullLogger<AprsIsConnection>.Instance, mockTcpConnection.Object, connectionShouldDispose))
            {
            }

            mockTcpConnection.Verify(mock => mock.Dispose(), connectionShouldDispose ? Times.Once : Times.Never);
        }
    }
}
