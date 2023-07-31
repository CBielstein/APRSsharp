namespace AprsSharpUnitTests.KissTnc;

using AprsSharp.KissTnc;
using AprsSharp.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

/// <summary>
/// Test <see cref="TcpTnc"/> class.
/// </summary>
[TestClass]
public class TcpTncUnitTests : BaseTNCUnitTests<TcpTnc>
{
    /// <inheritdoc/>
    public override TcpTnc BuildTestTnc()
    {
        var mockConn = new Mock<ITcpConnection>();
        mockConn.SetupGet(mock => mock.Connected).Returns(true);
        mockConn.Setup(mock => mock.AsyncReceive(It.IsAny<HandleReceivedBytes>()));
        mockConn.Setup(mock => mock.SendBytes(It.IsAny<byte[]>()));
        mockConn.Setup(mock => mock.SendString(It.IsAny<string>()));

        return new TcpTnc(mockConn.Object, 0);
    }
}
