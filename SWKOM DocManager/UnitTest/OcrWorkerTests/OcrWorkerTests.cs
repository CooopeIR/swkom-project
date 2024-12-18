using Elastic.Clients.Elasticsearch.MachineLearning;
using GemBox.Document;
using ImageMagick;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.TestPlatform.Utilities.Helpers.Interfaces;
using Moq;
using NUnit.Framework;
using OCRWorker;
using OCRWorker.ProcessLibrary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.OCRWorkerTests
{
    [TestFixture]
    public class OcrWorkerTests
    {
        private Mock<IProcessFactory> _mockProcessFactory;
        private Mock<IConnectionFactory> _mockConnectionFactory;
        private Mock<IConnection> _mockConnection;
        private Mock<IModel> _mockChannel;
        private OcrWorker _ocrWorker;

        [SetUp]
        public void SetUp()
        {
            _mockProcessFactory = new Mock<IProcessFactory>();
            _mockConnectionFactory = new Mock<IConnectionFactory>();
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();

            _mockConnectionFactory.Setup(factory => factory.CreateConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(conn => conn.CreateModel()).Returns(_mockChannel.Object);

            _ocrWorker = new OcrWorker(_mockProcessFactory.Object, _mockConnectionFactory.Object);
        }

        [Test]
        public void ConnectToRabbitMQ_ShouldCreateQueues_WhenConnectionSucceeds()
        {
            // Arrange
            _mockConnection.Setup(conn => conn.IsOpen).Returns(true);

            // Act
            _ocrWorker.ConnectToRabbitMQ();

            // Assert
            _mockChannel.Verify(channel => channel.QueueDeclare("file_queue", false, false, false, null), Times.Once);
            _mockChannel.Verify(channel => channel.QueueDeclare("ocr_result_queue", false, false, false, null), Times.Once);
        }

        [Test]
        public void ConnectToRabbitMQ_ShouldThrowException_WhenConnectionFails()
        {
            // Arrange
            _mockConnectionFactory.Setup(factory => factory.CreateConnection()).Throws(new Exception("Connection failed"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _ocrWorker.ConnectToRabbitMQ());
            Assert.That(ex.Message, Is.EqualTo("Konnte keine Verbindung zu RabbitMQ herstellen, alle Versuche fehlgeschlagen."));
        }

        public class DefaultProcessStartInfoFactory : IProcessStartInfoFactory
        {
            public ProcessStartInfo Create(string fileName, string arguments)
            {
                return new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
        }

        /*[Test]
        public void PerformOcr_ShouldExtractTextFromFile()
        {
            // Arrange
            var testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "test-file.pdf");
            //var testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "test-file.jpg");
            if (!File.Exists(testFilePath))
            {
                ComponentInfo.SetLicense("FREE - LIMITED - KEY");
                var document = new DocumentModel();
                document.Sections.Add(
                    new Section(document,
                        new Paragraph(document, "This is a test PDF file for OCR testing.")));
                document.Save(testFilePath);
            }

            // Sicherstellen, dass die Testdatei existiert
            Assert.IsTrue(File.Exists(testFilePath), "The test file does not exist at the specified path.");

            _mockConnection.Setup(conn => conn.CreateModel()).Returns(_mockChannel.Object);

            var mockConsumer = new EventingBasicConsumer(_mockChannel.Object);

            // Setup für BasicConsume (simulieren, dass Consumer registriert wird)
            _mockChannel
                .Setup(channel => channel.BasicConsume(
                    It.IsAny<string>(),  // Queue-Name
                    It.IsAny<bool>(),    // Auto-Ack
                    It.IsAny<string>(),  // Consumer-Tag
                    It.IsAny<bool>(),    // NoLocal
                    It.IsAny<bool>(),    // Exclusive
                    It.IsAny<IDictionary<string, object>>(), // Arguments
                    It.IsAny<IBasicConsumer>())) // Consumer
                .Returns("consumer-tag"); // Simulierter Rückgabewert

            _mockConnection.Setup(conn => conn.IsOpen).Returns(true);

            // Verwende die Dummy-Implementierung oder den Mock
            var processFactory = new ProcessFactory(new DefaultProcessStartInfoFactory());

            var ocrWorker = new OcrWorker(processFactory, Mock.Of<IConnectionFactory>()); // Verbindung zu RabbitMQ wird nicht benötigt
            ocrWorker.Initialize();
            //ocrWorker.Start();

            // Act
            string result = ocrWorker.PerformOcr(testFilePath);

            // Assert
            Assert.IsNotNull(result, "The OCR result should not be null.");
            Assert.IsNotEmpty(result, "The OCR result should not be empty.");
            Console.WriteLine("OCR Extracted Text: " + result);

            // Cleanup
            // Temporäre Dateien im Test löschen (nicht nötig, da `PerformOcr` sie löscht).
        }*/

        [Test]
        public void PerformOcr_ShouldHandleException_WhenProcessingFails()
        {
            // Arrange
            string mockFilePath = "mockFilePath";
            _mockProcessFactory.Setup(factory => factory.CreateProcess(It.IsAny<string>(), It.IsAny<string>()))
                               .Throws(new Exception("Processing error"));

            // Act
            string result = _ocrWorker.PerformOcr(mockFilePath);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Start_ShouldConsumeMessagesAndProcessOcr()
        {
            // Arrange
            _mockConnection.Setup(conn => conn.CreateModel()).Returns(_mockChannel.Object);

            var mockConsumer = new EventingBasicConsumer(_mockChannel.Object);

            // Setup für BasicConsume (simulieren, dass Consumer registriert wird)
            _mockChannel
                .Setup(channel => channel.BasicConsume(
                    It.IsAny<string>(),  // Queue-Name
                    It.IsAny<bool>(),    // Auto-Ack
                    It.IsAny<string>(),  // Consumer-Tag
                    It.IsAny<bool>(),    // NoLocal
                    It.IsAny<bool>(),    // Exclusive
                    It.IsAny<IDictionary<string, object>>(), // Arguments
                    It.IsAny<IBasicConsumer>())) // Consumer
                .Returns("consumer-tag"); // Simulierter Rückgabewert

            // Act
            _mockConnection.Setup(conn => conn.IsOpen).Returns(true);
            _ocrWorker.Initialize();
            _ocrWorker.Start();

            // Assert
            _mockChannel.Verify(channel => channel.BasicConsume(
                "file_queue", // Erwarteter Queue-Name
                true,         // Erwartetes Auto-Ack
                It.IsAny<string>(),  // Consumer-Tag
                false,        // NoLocal
                false,        // Exclusive
                null,         // Keine speziellen Argumente
                It.IsAny<IBasicConsumer>()), // Übergebenes Consumer-Objekt
                Times.Once);
        }

        [Test]
        public void InitializeRabbitMQConnection()
        {
            // Arrange
            _mockConnection.Setup(conn => conn.IsOpen).Returns(true);

            // Act
            _ocrWorker.Initialize();

            // Assert
            _mockChannel.Verify(channel => channel.QueueDeclare("file_queue", false, false, false, null), Times.Once);
            _mockChannel.Verify(channel => channel.QueueDeclare("ocr_result_queue", false, false, false, null), Times.Once);
        }

        [Test]
        public void Dispose_ShouldCloseChannelAndConnection()
        {
            // Arrange
            _mockConnection.Setup(conn => conn.IsOpen).Returns(true);
            _ocrWorker.ConnectToRabbitMQ();

            // Act
            _ocrWorker.Dispose();

            // Assert
            _mockChannel.Verify(channel => channel.Close(), Times.Once);
            _mockConnection.Verify(conn => conn.Close(), Times.Once);
        }

        [Test]
        public void GetChannelFunction()
        {
            // Arrange
            _mockConnection.Setup(conn => conn.IsOpen).Returns(true);

            // Act
            _ocrWorker.Initialize();

            // Assert
            Assert.That(_ocrWorker.GetChannel(), Is.Not.Null);
            _mockChannel.Verify(channel => channel.QueueDeclare("file_queue", false, false, false, null), Times.Once);
            _mockChannel.Verify(channel => channel.QueueDeclare("ocr_result_queue", false, false, false, null), Times.Once);
        }
    }
}