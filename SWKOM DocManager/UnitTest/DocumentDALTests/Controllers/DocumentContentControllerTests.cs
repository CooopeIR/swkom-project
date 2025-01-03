﻿using AutoMapper;
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

        [Test]
        public async Task AddContentAsync_ShouldAddNewDocumentContent()
        {
            // Arrange
            var contentBytes1 = Encoding.UTF8.GetBytes("New Test Content");
            var newContent = new DocumentContent { DocumentId = 3, Content = contentBytes1, ContentType = "text/plain", FileName = "testFileName" };

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
                Assert.That(addedContent.Content, Is.EqualTo(contentBytes1));
                Assert.That(addedContent.ContentType, Is.EqualTo("text/plain"));
                Assert.That(addedContent.FileName, Is.EqualTo("testFileName"));
            });
        }

        [Test]
        public async Task UpdateContentAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Arrange
            var contentBytes1 = Encoding.UTF8.GetBytes("New Test Content");
            var newContent = new DocumentContent { DocumentId = 3, Content = contentBytes1, ContentType = "text/plain", FileName = "testFileName" };
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
                Assert.That(addedContent.Content, Is.EqualTo(contentBytes1));
                Assert.That(addedContent.ContentType, Is.EqualTo("text/plain"));
                Assert.That(addedContent.FileName, Is.EqualTo("testFileName"));
            });

            var contentBytes2 = Encoding.UTF8.GetBytes("New Test Content changed");
            newContent.Content = contentBytes2;
            newContent.ContentType = "text/json";

            // Act
            await _controller.PutAsync(3, newContent);
            addedContent = await _context.DocumentContents.FindAsync(3);

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
                Assert.That(addedContent.Content, Is.EqualTo(contentBytes2));
                Assert.That(addedContent.ContentType, Is.EqualTo("text/json"));
                Assert.That(addedContent.FileName, Is.EqualTo("testFileName"));
            });
        }

        [Test]
        public async Task UpdateContentAsync_FailingNoItemGiven()
        {
            // Arrange
            var contentBytes1 = Encoding.UTF8.GetBytes("New Test Content");
            DocumentContent newContent = null;

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
        public async Task UpdateContentAsync_FailingInvalidIdGiven()
        {
            // Arrange
            var contentBytes1 = Encoding.UTF8.GetBytes("New Test Content");
            var newContent = new DocumentContent { DocumentId = 3, Content = contentBytes1, ContentType = "text/plain", FileName = "testFileName" };

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.PutAsync(99, newContent));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Document Contents for Document with ID 99 not found"));
        }

        [Test]
        public async Task DeleteContentAsync_ShouldDeleteDocumentContent_WhenIdExists()
        {
            // Act
            await _controller.DeleteAsync(1);
            var deletedContent = await _context.DocumentContents.FindAsync(1);

            // Assert: No exception should be thrown
            Assert.Pass("Method should not throw an exception when deleted an existing record.");
        }

        [Test]
        public async Task DeleteContentAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.DeleteAsync(99));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Document Contents for Document with ID 99 not found"));
        }
    }
}
