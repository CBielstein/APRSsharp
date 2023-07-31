namespace AprsSharpUnitTests.Protocols;

using AprsSharp.KissTnc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

/// <summary>
/// Test <see cref="SerialTNC"/> class.
/// </summary>
[TestClass]
public class SerialTNCUnitTests : BaseTNCUnitTests<SerialTNC>
{
    /// <inheritdoc/>
    public override SerialTNC BuildTestTnc()
    {
        var mockPort = new Mock<ISerialConnection>();
        mockPort.SetupGet(mock => mock.IsOpen).Returns(false);
        mockPort.Setup(mock => mock.Open());
        mockPort.Setup(mock => mock.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()));

        return new SerialTNC(mockPort.Object, 0);
    }
}
