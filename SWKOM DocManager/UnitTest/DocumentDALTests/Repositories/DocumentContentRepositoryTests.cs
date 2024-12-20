﻿using DocumentDAL.Data;
using DocumentDAL.Entities;
using DocumentDAL.Repositories;

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.DocumentDALTests.Repositories
{
    public class DocumentContentRepositoryTests
    {
        private DocumentContext _context;
        private DocumentContentRepository _repository;

        [SetUp]
        public void SetUp()
        {
            // Create an in-memory database for testing purposes
            var options = new DbContextOptionsBuilder<DocumentContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DocumentContext(options);
            _repository = new DocumentContentRepository(_context);

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
            var result = await _repository.GetAllContentAsync();

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
            var result = await _repository.GetContentByIdAsync(1);

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
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetContentByIdAsync(99));
            Assert.That(ex.Message, Is.EqualTo("Document Contents for Document with ID 99 not found"));
        }

        [Test]
        public async Task AddContentAsync_ShouldAddNewDocumentContent()
        {
            // Arrange
            var contentBytes1 = Encoding.UTF8.GetBytes("New Test Content");
            var newContent = new DocumentContent { DocumentId = 3, Content = contentBytes1 };

            // Act
            var result = await _repository.AddContentAsync(newContent);
            var addedContent = await _context.DocumentContents.FindAsync(3);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(addedContent, Is.Not.Null);
                Assert.That(addedContent.DocumentId, Is.EqualTo(3));
                Assert.That(addedContent.Content, Is.EqualTo(contentBytes1));
            });
        }

        [Test]
        public async Task DeleteContentAsync_ShouldDeleteDocumentContent_WhenIdExists()
        {
            // Act
            await _repository.DeleteContentAsync(1);
            var deletedContent = await _context.DocumentContents.FindAsync(1);

            // Assert
            Assert.That(deletedContent, Is.Null);
        }

        [Test]
        public async Task DeleteContentAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Act
            await _repository.DeleteContentAsync(99);

            // Assert: No exception should be thrown
            Assert.Pass("Method should not throw an exception when trying to delete a non-existing record.");
        }

        [Test]
        public async Task UpdateContentAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Arrange
            var contentBytes1 = Encoding.UTF8.GetBytes("New Test Content");
            var newContent = new DocumentContent { DocumentId = 3, Content = contentBytes1 };
            var result = await _repository.AddContentAsync(newContent);
            var contentBytes2 = Encoding.UTF8.GetBytes("New Test Content updated");
            newContent.Content = contentBytes2;

            // Act
            await _repository.UpdateContentAsync(newContent);
            var addedContent = await _context.DocumentContents.FindAsync(3);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(addedContent, Is.Not.Null);
                Assert.That(addedContent.DocumentId, Is.EqualTo(3));
                Assert.That(addedContent.Content, Is.EqualTo(contentBytes2));
            });
        }
    }
}
