using ImageMagick;
using Microsoft.VisualStudio.TestPlatform.Utilities.Helpers.Interfaces;
using Moq;
using NUnit.Framework;
using OCRWorker;
using OCRWorker.ProcessLibrary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace OCRWorker.Tests
{
    [TestFixture]
    public class OcrWorkerTests
    {
        private Mock<IProcessFactory> _processFactoryMock;
        private Mock<IProcessStartInfoFactory> _psiFactoryMock;
        private Mock<IConnectionFactory> _connectionFactoryMock;
        private Mock<IModel> _channelMock;
        private Mock<IModel> _connectionMock;


        // TODO: Change from Unit Tests to Integration Tests -> disregard Mocking of dependencies and use real instances


        //[SetUp]
        //public void Setup()
        //{

        //    //var imageMock = new Mock<MagickImage>("dummy-path"); // Assuming a file path is passed
        //    //imageMock.Setup(img => img.Density).Returns(new Density(300, 300)); // Mock the Density property
        //    //imageMock.Setup(img => img.Format).Returns(MagickFormat.Png); // Mock the Format property
        //    //imageMock.Setup(img => img.Contrast()).Verifiable(); // Mock the Contrast method call
        //    //imageMock.Setup(img => img.Sharpen()).Verifiable(); // Mock the Sharpen method call
        //    //imageMock.Setup(img => img.Despeckle()).Verifiable(); // Mock the Despeckle method call

        //    //imageMock.Setup(img => img.Write(It.IsAny<string>())).Verifiable(); // Just verify the Write call
        //    // Create a temporary real image for testing
        //    using (var image = new MagickImage(MagickColors.White, 100, 100))
        //    {
        //        image.Density = new Density(300, 300); // Set resolution
        //        image.Format = MagickFormat.Png;
        //        image.Write(tempImagePath);

        //        // Mocking the MagickImageCollection to return a mocked image
        //        var imageCollectionMock = new Mock<MagickImageCollection>();
        //        imageCollectionMock.Setup(collection => collection.GetEnumerator())
        //            .Returns(new List<MagickImage> { image }.GetEnumerator()); // Return the mocked image

        //        // Mocking the Write method to avoid writing files
        //        image.Setup(img => img.Write(It.IsAny<string>())).Verifiable(); // Just verify the Write call
        //    }


        //    // Setup PSI Factory to return a ProcessStartInfo Object with Arguments
        //    _psiFactoryMock = new Mock<IProcessStartInfoFactory>();
        //    var processStartInfoMock = new Mock<ProcessStartInfo>("tesseract", "fileName stdout -l")
        //    {
        //        CallBase = true // Optional if you want the actual behavior of ProcessStartInfo
        //    };

        //    _psiFactoryMock.Setup(psi => psi.Create(It.IsAny<string>(), It.IsAny<string>()))
        //        .Returns(() => processStartInfoMock);

        //    // Setup ProcessFactory to Return a new ProcessWrapper
        //    _processFactoryMock = new Mock<IProcessFactory>(_psiFactoryMock.Object);

        //    var processWrapperMock = new Mock<ProcessWrapper>();
        //    processWrapperMock.Setup(p => p.GetOutput()).Returns(() => "Das ist ein Test");
        //    _processFactoryMock.Setup(pf => pf.CreateProcess(It.IsAny<string>(), It.IsAny<string>())).Returns(() => processWrapperMock);

        //}

        [Test]
        public void PerformOcr_ReturnsText_WithRealMagickImage()
        {
            // Arrange
            var expectedText = "Das ist ein Test"; // Expected OCR output
            var tempImagePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");

            try
            {
                

                // Mock the ProcessWrapper to simulate OCR output
                var processWrapperMock = new Mock<IProcess>();
                processWrapperMock.Setup(p => p.GetOutput()).Returns(expectedText);

                _processFactoryMock.Setup(pf => pf.CreateProcess(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(processWrapperMock.Object);

                // Instantiate your class under test
                var ocrWorker = new OcrWorker(_processFactoryMock.Object, _connectionFactoryMock.Object);

                // Act
                var result = ocrWorker.PerformOcr(tempImagePath);

                // Assert
                Assert.AreEqual(expectedText, result, "The OCR result text does not match the expected output.");

                // Verify process creation
                _processFactoryMock.Verify(pf => pf.CreateProcess(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "Process creation was not called as expected.");
            }
            finally
            {
                // Cleanup the temporary file
                if (File.Exists(tempImagePath))
                {
                    File.Delete(tempImagePath);
                }
            }
        }
    }
}