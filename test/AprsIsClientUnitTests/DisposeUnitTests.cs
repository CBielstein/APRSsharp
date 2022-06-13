namespace AprsSharpUnitTests.Connections.AprsIs
{
    using AprsSharp.Connections.AprsIs;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="AprsIsClient.Dispose()"/>.
    /// </summary>
    [Collection(nameof(TimedTestCollection))]
    public class DisposeUnitTests
    {
        /// <summary>
        /// Ensures <see cref="AprsIsClient.Dispose()"/> properly disposes
        /// the injected <see cref="ITcpConnection"/>.
        /// </summary>
        /// <param name="clientShouldDispose">`true` if the mock <see cref="ITcpConnection"/> should be disposed by the
        ///     <see cref="AprsIsClient"/> under test.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ProperlyHandlesInjectedTcpConnection(bool clientShouldDispose)
        {
            var mockTcpConnection = new Mock<ITcpConnection>();

            using (AprsIsClient connection = new AprsIsClient(NullLogger<AprsIsClient>.Instance, mockTcpConnection.Object, clientShouldDispose))
            {
            }

            mockTcpConnection.Verify(mock => mock.Dispose(), clientShouldDispose ? Times.Once : Times.Never);
        }
    }
}
