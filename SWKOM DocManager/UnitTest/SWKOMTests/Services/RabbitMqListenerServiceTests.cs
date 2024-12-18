using DocumentDAL.Entities;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SWKOM.Services;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest.SWKOMTests.Services
{
    public class RabbitMqListenerServiceTests
    {
        private Mock<IConnectionFactory> _mockConnectionFactory;
        private Mock<IConnection> _mockConnection;
        private Mock<IModel> _mockChannel;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<HttpClient> _mockHttpClient;
        private RabbitMqListenerService _service;

        /*[SetUp]
        public void SetUp()
        {
            _mockConnectionFactory = new Mock<IConnectionFactory>();
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpClient = new Mock<HttpClient>();

            // Set up the mock to return the mocked connection and channel
            _mockConnectionFactory.Setup(factory => factory.CreateConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(conn => conn.CreateModel()).Returns(_mockChannel.Object);

            // Set up the mock HttpClient factory to return the mocked HttpClient
            _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(_mockHttpClient.Object);

            // Create an instance of the service
            _service = new RabbitMqListenerService(_mockHttpClientFactory.Object, _mockConnectionFactory.Object);
        }*/

        [TearDown]
        public void TearDown()
        {
            // Dispose of IDisposable objects if needed
            _mockConnection?.Object?.Dispose();
            _mockChannel?.Object?.Dispose();
        }
    }
}
