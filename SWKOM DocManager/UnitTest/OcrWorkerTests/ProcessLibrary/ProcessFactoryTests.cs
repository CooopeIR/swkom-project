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

namespace UnitTest.OcrWorkerTests.ProcessLibrary
{
    [TestFixture]
    public class ProcessFactoryTests
    {
        private Mock<IProcessStartInfoFactory> _mockPsiFactory;
        private ProcessFactory _processFactory;

        [SetUp]
        public void SetUp()
        {
            _mockPsiFactory = new Mock<IProcessStartInfoFactory>();
            _processFactory = new ProcessFactory(_mockPsiFactory.Object);
        }

        [Test]
        public void CreateProcess_ShouldReturnProcessWrapper_WhenValidArgumentsArePassed()
        {
            // Arrange
            var fileName = "tesseract";
            var arguments = "C:\\temp\\image.png stdout -l eng";

            // Erstellen des erwarteten ProcessStartInfo-Objekts
            var expectedPsi = new ProcessStartInfo(fileName, arguments);
            _mockPsiFactory.Setup(x => x.Create(fileName, arguments)).Returns(expectedPsi);

            // Act
            IProcess process = _processFactory.CreateProcess(fileName, arguments);

            // Assert
            Assert.That(process, Is.InstanceOf<ProcessWrapper>()); // Stellen sicher, dass der zurückgegebene Prozess ein ProcessWrapper ist
            _mockPsiFactory.Verify(x => x.Create(fileName, arguments), Times.Once); // Sicherstellen, dass Create einmal aufgerufen wurde
        }

        [Test]
        public void CreateProcess_ShouldReturnProcessWrapperWithCorrectProcessStartInfo()
        {
            // Arrange
            var fileName = "tesseract";
            var arguments = "C:\\temp\\image.png stdout -l eng";
            var expectedPsi = new ProcessStartInfo(fileName, arguments);

            // Setup the mock factory to return the expected ProcessStartInfo
            _mockPsiFactory.Setup(x => x.Create(fileName, arguments)).Returns(expectedPsi);

            // Act
            var process = _processFactory.CreateProcess(fileName, arguments);

            // Assert
            var processWrapper = process as ProcessWrapper;
            Assert.That(processWrapper, Is.Not.Null); // Sicherstellen, dass der zurückgegebene Prozess ein ProcessWrapper ist

            // Zugriff auf die StartInfo des ProcessWrapper
            var actualPsi = processWrapper.StartInfo;

            // Überprüfen, dass die StartInfo die erwarteten Werte hat
            Assert.Multiple(() =>
            {
                Assert.That(actualPsi.FileName, Is.EqualTo(expectedPsi.FileName));
                Assert.That(actualPsi.Arguments, Is.EqualTo(expectedPsi.Arguments));
            });
        }
    }
}