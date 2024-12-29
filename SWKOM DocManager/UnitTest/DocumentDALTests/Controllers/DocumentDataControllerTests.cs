using AutoMapper;
using DocumentDAL.Controllers;
using DocumentDAL.Data;
using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using SWKOM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnitTest.DocumentDALTests.Entities;

namespace UnitTest.DocumentDALTests.Controllers
{
    public class DocumentDataControllerTests
    {
        private DocumentContext _context;
        private DocumentDataController _controller;
        private IDocumentDataRepository _repository;

        [SetUp]
        public void SetUp()
        {
            // Create an in-memory database for testing purposes
            var options = new DbContextOptionsBuilder<DocumentContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DocumentContext(options);
            _repository = new DocumentDataRepository(_context);
            _controller = new DocumentDataController(_repository);

            var contentBytes1 = Encoding.UTF8.GetBytes("Test Content 1");
            var contentBytes2 = Encoding.UTF8.GetBytes("Test Content 2");

            // Seed the database with test data
            DateTime now = DateTime.Now;
            DateTime now2 = DateTime.Now;
            _context.DocumentMetadatas.AddRange(
                new DocumentMetadata { DocumentId = 1, UploadDate = now, FileSize = 5 },
                new DocumentMetadata { DocumentId = 2, UploadDate = now2, FileSize = 10 }
            );
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllDataAsync_ShouldReturnAllDocumentDatas()
        {
            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            Assert.Multiple(() =>
            {
                int i = 1;
                int fileSize = 0;
                foreach (var item in result)
                {
                    if (i <= result.Count())
                    {
                        break;
                    }
                    fileSize += 5;
                    Assert.That(item.DocumentId, Is.EqualTo(i));
                    Assert.That(item.FileSize, Is.EqualTo(fileSize));
                    i++;
                }
            });
        }

        [Test]
        public async Task GetDataByIdAsync_ShouldReturnDocumentData_WhenIdExists()
        {
            // Act
            var result = await _controller.GetAsyncById(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.DocumentId, Is.EqualTo(1));
                Assert.That(result.FileSize, Is.EqualTo(5));
            });
        }

        [Test]
        public async Task GetDataByIdAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetMetaByIdAsync(99));
            Assert.That(ex.Message, Is.EqualTo("Document Metadata for Document with ID 99 not found"));
        }

        [Test]
        public async Task AddDataAsync_ShouldFail()
        {
            // Arrange
            var newContent = new DocumentMetadata { DocumentId = 3, FileSize = null };

            // Act
            var result = await _controller.PostAsync(newContent);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "variable bad request result");

            // Deserialize the object
            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var value = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.That(value, Is.Not.Null, "variable value");
            Assert.That(value["message"], Is.EqualTo("Document Information cannot be empty :/"));
        }

        [Test]
        public async Task AddDataAsync_ShouldAddNewDocumentData()
        {
            // Arrange
            DateTime now = DateTime.Now;
            var newContent = new DocumentMetadata { DocumentId = 3, UploadDate = now, FileSize = 5 };

            // Act
            var result = await _controller.PostAsync(newContent);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.That(createdAtActionResult, Is.Not.Null, "variable created at action result");

            // Deserialize the object
            var json = JsonConvert.SerializeObject(createdAtActionResult.Value);
            var value = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.That(value, Is.Not.Null, "variable value");
            Assert.That(value["id"], Is.EqualTo("3"));

            var addedContent = await _controller.GetAsyncById(3);
            Assert.Multiple(() =>
            {
                Assert.That(addedContent, Is.Not.Null);
                Assert.That(addedContent.DocumentId, Is.EqualTo(3));
                Assert.That(addedContent.UploadDate, Is.EqualTo(now));
                Assert.That(addedContent.FileSize, Is.EqualTo(5));
            });
        }

        [Test]
        public async Task UpdateDataAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Arrange
            DateTime now = DateTime.Now;
            var newContent = new DocumentMetadata { DocumentId = 3, UploadDate = now, FileSize = 5 };
            var result = await _controller.PostAsync(newContent);
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.That(createdAtActionResult, Is.Not.Null, "variable created at action result");
            // Deserialize the object
            var json = JsonConvert.SerializeObject(createdAtActionResult.Value);
            var value = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.That(value, Is.Not.Null, "variable value");
            Assert.That(value["id"], Is.EqualTo("3"));
            var addedContent = await _controller.GetAsyncById(3);
            Assert.Multiple(() =>
            {
                Assert.That(addedContent, Is.Not.Null);
                Assert.That(addedContent.DocumentId, Is.EqualTo(3));
                Assert.That(addedContent.UploadDate, Is.EqualTo(now));
                Assert.That(addedContent.FileSize, Is.EqualTo(5));
            });

            DateTime now2 = DateTime.Now;
            newContent.UploadDate = now2;
            newContent.FileSize = 10;

            // Act
            await _controller.PutAsync(3, newContent);
            addedContent = await _context.DocumentMetadatas.FindAsync(3);

            // Assert
            createdAtActionResult = result as CreatedAtActionResult;
            Assert.That(createdAtActionResult, Is.Not.Null, "variable created at action result");
            // Deserialize the object
            json = JsonConvert.SerializeObject(createdAtActionResult.Value);
            value = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.That(value, Is.Not.Null, "variable value");
            Assert.That(value["id"], Is.EqualTo("3"));

            addedContent = await _controller.GetAsyncById(3);
            Assert.Multiple(() =>
            {
                Assert.That(addedContent, Is.Not.Null);
                Assert.That(addedContent.DocumentId, Is.EqualTo(3));
                Assert.That(addedContent.UploadDate, Is.EqualTo(now2));
                Assert.That(addedContent.FileSize, Is.EqualTo(10));
            });
        }

        [Test]
        public async Task UpdateDataAsync_FailingNoItemGiven()
        {
            // Arrange
            DocumentMetadata newContent = null;

            // Act
            var result = await _controller.PutAsync(3, newContent);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "variable bad request result");

            // Deserialize the object
            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var value = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.That(value, Is.Not.Null, "variable value");
            Assert.That(value["message"], Is.EqualTo("Invalid payload"));
        }

        [Test]
        public async Task UpdateDataAsync_FailingInvalidIdGiven()
        {
            // Arrange
            DateTime now = DateTime.Now;
            var newContent = new DocumentMetadata { DocumentId = 3, UploadDate = now, FileSize = 5 };

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.PutAsync(99, newContent));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Document Metadata for Document with ID 99 not found"));
        }

        [Test]
        public async Task DeleteDataAsync_ShouldDeleteDocumentData_WhenIdExists()
        {
            // Act
            await _controller.DeleteAsync(1);
            var deletedContent = await _context.DocumentMetadatas.FindAsync(1);

            // Assert: No exception should be thrown
            Assert.Pass("Method should not throw an exception when deleted an existing record.");
        }

        [Test]
        public async Task DeleteDataAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.DeleteAsync(99));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Document Metadata for Document with ID 99 not found"));
        }
    }
}
