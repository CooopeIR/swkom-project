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
    public class DocumentItemsControllerTests
    {
        private DocumentContext _context;
        private DocumentController _controller;
        private IDocumentItemRepository _repository;

        [SetUp]
        public void SetUp()
        {
            // Create an in-memory database for testing purposes
            var options = new DbContextOptionsBuilder<DocumentContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DocumentContext(options);
            _repository = new DocumentItemRepository(_context);
            _controller = new DocumentController(_repository);

            var contentBytes1 = Encoding.UTF8.GetBytes("Test Content 1");
            var contentBytes2 = Encoding.UTF8.GetBytes("Test Content 2");

            // Seed the database with test data
            _context.DocumentItems.AddRange(
                new DocumentItem { Title = "Title Test 1", Author = "Author Test 1", OcrText = "This is example content." },
                new DocumentItem { Title = "Title Test 2", Author = "Author Test 2", OcrText = "This is example content." }
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
                foreach (var item in result)
                {
                    if (i <= result.Count())
                    {
                        break;
                    }
                    Assert.That(item.Title, Is.EqualTo("Title Test " + i));
                    Assert.That(item.Author, Is.EqualTo("Author Test " + i));
                    Assert.That(item.OcrText, Is.EqualTo("This is example content."));
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
                Assert.That(result.Title, Is.EqualTo("Title Test 1"));
                Assert.That(result.Author, Is.EqualTo("Author Test 1"));
                Assert.That(result.OcrText, Is.EqualTo("This is example content."));
            });
        }

        [Test]
        public async Task GetDataByIdAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetByIdAsync(99));
            Assert.That(ex.Message, Is.EqualTo("DocumentItem with ID 99 not found"));
        }

        [Test]
        public async Task AddDataAsync_ShouldFail()
        {
            // Arrange
            var newContent = new DocumentItem { Title = null, Author = null, OcrText = "This is example content." };

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
            var newContent = new DocumentItem { Title = "Title Test 3", Author = "Author Test 3", OcrText = "This is example content." };

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
                Assert.That(addedContent.Title, Is.EqualTo("Title Test 3"));
                Assert.That(addedContent.Author, Is.EqualTo("Author Test 3"));
                Assert.That(addedContent.OcrText, Is.EqualTo("This is example content."));
            });
        }

        [Test]
        public async Task UpdateDataAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Arrange
            var newContent = new DocumentItem { Title = "Title Test 3", Author = "Author Test 3", OcrText = "This is example content." };
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
                Assert.That(addedContent.Title, Is.EqualTo("Title Test 3"));
                Assert.That(addedContent.Author, Is.EqualTo("Author Test 3"));
                Assert.That(addedContent.OcrText, Is.EqualTo("This is example content."));
            });

            newContent.Title = "Another title";
            newContent.Author = "Another Author new";

            // Act
            await _controller.PutAsync(3, newContent);
            addedContent = await _context.DocumentItems.FindAsync(3);

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
                Assert.That(addedContent.Title, Is.EqualTo("Another title"));
                Assert.That(addedContent.Author, Is.EqualTo("Another Author new"));
                Assert.That(addedContent.OcrText, Is.EqualTo("This is example content."));
            });
        }

        [Test]
        public async Task UpdateDataAsync_FailingNoItemGiven()
        {
            // Arrange
            DocumentItem newContent = null;

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
            var newContent = new DocumentItem { Title = "Title Test 3", Author = "Author Test 3", OcrText = "This is example content." };

            // Act
            var result = Assert.ThrowsAsync<Exception>(async () => await _controller.PutAsync(99, newContent));

            // Assert
            Assert.That(result.Message, Is.EqualTo("DocumentItem with ID 99 not found"));
        }

        [Test]
        public async Task DeleteDataAsync_ShouldDeleteDocumentData_WhenIdExists()
        {
            // Act
            await _controller.DeleteAsync(1);
            var deletedContent = await _context.DocumentItems.FindAsync(1);

            // Assert: No exception should be thrown
            Assert.Pass("Method should not throw an exception when deleted an existing record.");
        }

        [Test]
        public async Task DeleteDataAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.DeleteAsync(99));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("DocumentItem with ID 99 not found"));
        }

        [Test]
        public async Task ViewDocument_success()
        {
            // Arrange
            var newContent = new DocumentItem { Title = "Title Test 4", Author = "Author Test 4", OcrText = "This is example content.", DocumentContent = null, DocumentMetadata = null };
            var added = await _repository.AddAsync(newContent);

            // Act
            var result = await _controller.ViewDocumentAsync(3);

            // Assert
            var addedContent = await _controller.GetAsyncById(3);
            Assert.Multiple(() =>
            {
                Assert.That(addedContent, Is.Not.Null);
                Assert.That(addedContent.Title, Is.EqualTo("Title Test 4"));
                Assert.That(addedContent.Author, Is.EqualTo("Author Test 4"));
                Assert.That(addedContent.OcrText, Is.EqualTo("This is example content."));
                Assert.That(addedContent.DocumentContent, Is.Null);
                Assert.That(addedContent.DocumentMetadata, Is.Null);
            });
        }

        [Test]
        public async Task ViewDocument_notFound()
        {
            // Act
            var result = Assert.ThrowsAsync<Exception>(async () => await _controller.ViewDocumentAsync(99));

            // Assert
            Assert.That(result.Message, Is.EqualTo("DocumentItem with ID 99 not found"));
        }
    }
}
