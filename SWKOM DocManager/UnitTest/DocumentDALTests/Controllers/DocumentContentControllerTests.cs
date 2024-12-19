using AutoMapper;
using DocumentDAL.Controllers;
using DocumentDAL.Data;
using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Elastic.Clients.Elasticsearch;
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

namespace UnitTest.DocumentDALTests.Controllers
{
    public class DocumentContentControllerTests
    {
        private DocumentContext _context;
        private DocumentContentController _controller;
        private IDocumentContentRepository _repository;

        [SetUp]
        public void SetUp()
        {
            // Create an in-memory database for testing purposes
            var options = new DbContextOptionsBuilder<DocumentContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DocumentContext(options);
            _repository = new DocumentContentRepository(_context);
            _controller = new DocumentContentController(_repository);

            var contentBytes1 = Encoding.UTF8.GetBytes("Test Content 1");
            var contentBytes2 = Encoding.UTF8.GetBytes("Test Content 2");

            // Seed the database with test data
            _context.DocumentContents.AddRange(
                new DocumentContent { DocumentId = 1, Content = contentBytes1, FileName = "Test File 1" },
                new DocumentContent { DocumentId = 2, Content = contentBytes2, FileName = "Test File 2" }
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
        public async Task GetAllContentAsync_ShouldReturnAllDocumentContents()
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
                    Assert.That(item.FileName, Is.EqualTo("Test File " + i));
                    i++;
                }
            });
        }

        [Test]
        public async Task GetContentByIdAsync_ShouldReturnDocumentContent_WhenIdExists()
        {
            // Arrange
            var contentBytes1 = Encoding.UTF8.GetBytes("Test Content 1");

            // Act
            var result = await _controller.GetAsyncById(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.DocumentId, Is.EqualTo(1));
                Assert.That(result.Content, Is.EqualTo(contentBytes1));
            });
        }

        [Test]
        public async Task GetContentByIdAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.GetAsyncById(99));
            Assert.That(ex.Message, Is.EqualTo("Document Contents for Document with ID 99 not found"));
        }

        [Test]
        public async Task AddContentAsync_ShouldFail()
        {
            // Arrange
            var contentBytes1 = Encoding.UTF8.GetBytes("New Test Content");
            var newContent = new DocumentContent { DocumentId = 3, ContentType = null, FileName = null };

            // Act
            var ex = await _controller.PostAsync(newContent);

            // Assert
            Assert.That(ex, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = ex as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "variable bad request result");

            // Deserialize the object
            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var value = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.That(value, Is.Not.Null, "variable value");
            Assert.That(value["message"], Is.EqualTo("Document ContentType / Filename cannot be empty :/"));
        }
    }
}
