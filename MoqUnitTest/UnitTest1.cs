using NUnit.Framework;
using AprsIsConnection;
using Moq;
using FluentAssertions;
namespace MoqUnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Mock<ITcpConnection> mockITcpconnection = new Mock<ITcpConnection>();
            var tcpconnection  = new TcpConnection (mockITcpconnection.Object);
            tcpconnection.Should().NotBeNull();

            Assert.Pass();
        }
    }
}