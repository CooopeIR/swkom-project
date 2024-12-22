using DocumentDAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Moq;
using NUnit.Framework;
using SWKOM.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest.SWKOMTests.Services
{
    public class FileServiceTests
    {
        private Mock<IMinioClient> _mockMinioClient;
        private Mock<ILogger<FileService>> _mockLogger;
        private FileService _fileService;

        [SetUp]
        public void SetUp()
        {
            _mockMinioClient = new Mock<IMinioClient>();
            _mockLogger = new Mock<ILogger<FileService>>();
            _fileService = new FileService(_mockLogger.Object);

            // Use a private setter to inject the mock MinioClient
            typeof(FileService).GetField("_minioClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_fileService, _mockMinioClient.Object);
        }

        [Test]
        public async Task UploadFile_FileIsNull_ThrowsException()
        {
            // Arrange
            IFormFile file = null;

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _fileService.UploadFile(file));
            Assert.That(ex.Message, Is.EqualTo("File is empty or missing"));
        }
    }
}
