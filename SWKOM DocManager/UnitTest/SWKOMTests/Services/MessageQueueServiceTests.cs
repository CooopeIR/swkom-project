using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using SWKOM.Services;
using System;
using System.Text;

namespace UnitTest.SWKOMTests.Services
{
    public class MessageQueueServiceTests : IDisposable
    {
        private Mock<IConnection> _mockConnection;
        private Mock<IModel> _mockChannel;
        private Mock<IConnectionFactory> _mockFactory;
        private MessageQueueService _service;

        [SetUp]
        public void SetUp()
        {
            // Arrange mocks
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();
            _mockFactory = new Mock<IConnectionFactory>();

            // Set default behavior for IsOpen
            _mockChannel.Setup(c => c.IsOpen).Returns(true);
            _mockConnection.Setup(c => c.IsOpen).Returns(true);

            // Set up the factory to return the mock connection
            _mockFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);

            // Set up the connection to return the mock channel
            _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

            // Inject mocks into MessageQueueService
            _service = new MessageQueueService(_mockConnection.Object, _mockChannel.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }

        [Test]
        public void Dispose_ClosesChannelAndConnection()
        {
            // Act
            _service.Dispose();

            // Assert
            _mockChannel.Verify(c => c.Close(), Times.Once, "Channel.Close() should be called once.");
            _mockConnection.Verify(c => c.Close(), Times.Once, "Connection.Close() should be called once.");
        }

        [Test]
        public void Dispose_ChannelAlreadyClosed_DoesNotThrow()
        {
            // Arrange
            _mockChannel.Setup(c => c.IsOpen).Returns(false);

            // Act & Assert
            Assert.DoesNotThrow(() => _service.Dispose());
            _mockChannel.Verify(c => c.Close(), Times.Never, "Channel.Close() should not be called if channel is already closed.");
            _mockConnection.Verify(c => c.Close(), Times.Once, "Connection.Close() should be called even if channel is already closed.");
        }

        [Test]
        public void Dispose_ConnectionAlreadyClosed_DoesNotThrow()
        {
            // Arrange
            _mockConnection.Setup(c => c.IsOpen).Returns(false);

            // Act & Assert
            Assert.DoesNotThrow(() => _service.Dispose());
            _mockChannel.Verify(c => c.Close(), Times.Once, "Channel.Close() should still be called even if connection is already closed.");
            _mockConnection.Verify(c => c.Close(), Times.Never, "Connection.Close() should not be called if connection is already closed.");
        }

        public void Dispose()
        {
            _service?.Dispose();
        }
    }
}
