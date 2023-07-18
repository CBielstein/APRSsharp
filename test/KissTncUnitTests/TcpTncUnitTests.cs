namespace AprsSharpUnitTests.Protocols;

using AprsSharp.Protocols.KISS;
using AprsSharp.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

/// <summary>
/// Test <see cref="TcpTNC"/> class.
/// </summary>
[TestClass]
public class TcpTNCUnitTests : BaseTNCUnitTests<TcpTNC>
{
    /// <inheritdoc/>
    public override TcpTNC BuildTestTnc()
    {
        var mockConn = new Mock<ITcpConnection>();
        mockConn.SetupGet(mock => mock.Connected).Returns(true);
        mockConn.Setup(mock => mock.AsyncReceive(It.IsAny<HandleReceivedBytes>()));
        mockConn.Setup(mock => mock.SendBytes(It.IsAny<byte[]>()));
        mockConn.Setup(mock => mock.SendString(It.IsAny<string>()));

        return new TcpTNC(mockConn.Object, 0);
    }
}
