using Moq;
using NUnit.Framework;
using OCRWorker;
using OCRWorker.ProcessLibrary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

namespace OCRWorker.Tests
{
    [TestFixture]
    public class OcrWorkerTests
    {
        private Mock<IProcessFactory> _processFactoryMock;
        private Mock<IConnectionFactory> _connectionFactoryMock;
        private Mock<IModel> _channelMock;
        private OcrWorker _ocrWorker;

        [SetUp]
        public void Setup()
        {
            _processFactoryMock = new Mock<IProcessFactory>();
            _connectionFactoryMock = new Mock<IConnectionFactory>();

            // Create OcrWorker with mocks
            _ocrWorker = new OcrWorker(_processFactoryMock.Object, _connectionFactoryMock.Object);
        }

        [Test]
        public void Test_PerformOcr()
        {
            // Arrange
            var filePath = "test.png";
            var expectedText = "This is the extracted text";

            var processMock = new Mock<IProcess>();
            processMock.Setup(p => p.Start()).Verifiable();
            processMock.Setup(p => p.GetOutput()).Returns(expectedText);

            _processFactoryMock.Setup(f => f.CreateProcess(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(processMock.Object);

            // Act
            var actualText = _ocrWorker.PerformOcr(filePath);

            // Assert
            Assert.AreEqual(expectedText, actualText);
            processMock.Verify(p => p.Start(), Times.Once);
            processMock.Verify(p => p.GetOutput(), Times.Once);
        }
    }
}