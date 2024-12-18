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
    public class ProcessWrapperTests
    {
        private Mock<IProcessStartInfoFactory> _mockStartInfoFactory;
        private ProcessStartInfo _startInfo;
        private Mock<Process> _mockProcess;
        private Mock<StreamReader> _mockStreamReader;
        private ProcessWrapper _processWrapper;

        [SetUp]
        public void SetUp()
        {
            // Mock für IProcessStartInfoFactory
            _mockStartInfoFactory = new Mock<IProcessStartInfoFactory>();

            // Erstelle eine echte ProcessStartInfo-Instanz
            _startInfo = new ProcessStartInfo("test.exe", "test arguments")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Mock für IProcessStartInfoFactory, das die ProcessStartInfo erzeugt
            _mockStartInfoFactory.Setup(factory => factory.Create(It.IsAny<string>(), It.IsAny<string>()))
                                 .Returns(_startInfo);

            // Prozess-Wrapper wird nun mit dem Mock des StartInfoFactory erstellt
            _mockProcess = new Mock<Process>();

            // Wir können ProcessWrapper nun mit einem Mock von ProcessStartInfoFactory verwenden
            _processWrapper = new ProcessWrapper(_startInfo);

            _mockStreamReader = new Mock<StreamReader>(new MemoryStream());

            //_mockProcess.Setup(p => p.StandardOutput).Returns(_mockStreamReader.Object);
        }

        [Test]
        public void StartInfo_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var newStartInfo = new ProcessStartInfo("newTest.exe", "new arguments");

            // Act
            _processWrapper.StartInfo = newStartInfo;

            // Assert
            Assert.That(_processWrapper.StartInfo, Is.EqualTo(newStartInfo));
        }

        [Test]
        public void StartAndReadOutput_ShouldReturnExpectedOutput_WhenProcessIsRun()
        {
            // Arrange
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardOutput = true
            };
            var processWrapper = new ProcessWrapper(startInfo);

            // Act
            processWrapper.Start();
            var output = processWrapper.GetOutput();
            processWrapper.WaitForExit();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(output));
            Console.WriteLine(output); // Zeigt die tatsächliche Ausgabe
        }
    }
}