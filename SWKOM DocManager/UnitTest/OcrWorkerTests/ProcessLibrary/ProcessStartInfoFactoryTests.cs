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
    public class ProcessStartInfoFactoryTests
    {
        private IProcessStartInfoFactory _processStartInfoFactory;

        [SetUp]
        public void SetUp()
        {
            _processStartInfoFactory = new ProcessStartInfoFactory();
        }

        [Test]
        public void Create_ShouldReturnProcessStartInfoWithCorrectValues()
        {
            // Arrange
            var fileName = "tesseract";
            var arguments = "C:\\temp\\image.png stdout -l eng";

            // Act
            var processStartInfo = _processStartInfoFactory.Create(fileName, arguments);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(processStartInfo.FileName, Is.EqualTo(fileName));
                Assert.That(processStartInfo.Arguments, Is.EqualTo(arguments));
                Assert.That(processStartInfo.RedirectStandardOutput, Is.True);
                Assert.That(processStartInfo.UseShellExecute, Is.False);
                Assert.That(processStartInfo.CreateNoWindow, Is.True);
            });
        }
    }
}