namespace AprsIsUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AprsISLibrary;
    using Xunit;

    /// <summary>
    /// Unit tests for APRS IS library.
    /// </summary>
    public class UnitTest1
    {
        private readonly IList<string> tcpMessagesReceived = new List<string>();

        /// <summary>
        /// Receive raises event on TCP message received.
        /// </summary>
        [Fact]
        public void TestReceiveEvent()
        {
            // Setup
            // Create an APRS IS connection object.
            var arpsIs = new AprsISLib();
            arpsIs.ReceivedTcpMessage += TestTcpHandler;

            // Action
            // Receive some packets from it.
            arpsIs.Receive();

            // Assertions
            Assert.NotEmpty(tcpMessagesReceived);
        }

        private void TestTcpHandler(string tcpMessage)
        {
            tcpMessagesReceived.Add(tcpMessage);
        }
    }
}
