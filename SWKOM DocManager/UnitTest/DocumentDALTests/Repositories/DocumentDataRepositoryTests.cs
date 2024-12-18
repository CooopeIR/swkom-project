using DocumentDAL.Data;
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
    public class DocumentDataRepositoryTests
    {
        private DocumentContext _context;
        private DocumentDataRepository _repository;

        [SetUp]
        public void SetUp()
        {
            // Create an in-memory database for testing purposes
            var options = new DbContextOptionsBuilder<DocumentContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DocumentContext(options);
            _repository = new DocumentDataRepository(_context);

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
            _context.Dispose();
        }

        [Test]
        public async Task GetAllMetadataAsync_ShouldReturnAllDocumentMetadata()
        {
            // Act
            var result = await _repository.GetAllMetaAsync();

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
        public async Task GetMetadataByIdAsync_ShouldReturnDocumentMetadata_WhenIdExists()
        {
            // Arrange

            // Act
            var result = await _repository.GetMetaByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.DocumentId);
            Assert.AreEqual(5, result.FileSize);
        }

        [Test]
        public async Task GetMetadataByIdAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetMetaByIdAsync(99));
            Assert.AreEqual("Document Metadata for Document with ID 99 not found", ex.Message);
        }

        [Test]
        public async Task AddMetadataAsync_ShouldAddNewDocumentMetadata()
        {
            // Arrange
            var newContent = new DocumentMetadata { DocumentId = 3, FileSize = 15 };

            // Act
            var result = await _repository.AddMetaAsync(newContent);
            var addedContent = await _context.DocumentMetadatas.FindAsync(3);

            // Assert
            Assert.IsNotNull(addedContent);
            Assert.AreEqual(3, addedContent.DocumentId);
            Assert.AreEqual(15, addedContent.FileSize);
        }

        [Test]
        public async Task DeleteMetadataAsync_ShouldDeleteDocumentMetadata_WhenIdExists()
        {
            // Act
            await _repository.DeleteMetaAsync(1);
            var deletedContent = await _context.DocumentContents.FindAsync(1);

            // Assert
            Assert.IsNull(deletedContent);
        }

        [Test]
        public async Task DeleteMetadataAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Act
            await _repository.DeleteMetaAsync(99);

            // Assert: No exception should be thrown
            Assert.Pass("Method should not throw an exception when trying to delete a non-existing record.");
        }

        [Test]
        public async Task UpdateMetadataAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Arrange
            var newContent = new DocumentMetadata { Id = 6, DocumentId = 6, FileSize = 20 };
            var result = await _repository.AddMetaAsync(newContent);
            newContent.FileSize = 25;

            // Act
            await _repository.UpdateMetaAsync(newContent);
            var addedContent = await _context.DocumentMetadatas.FindAsync(6);

            // Assert
            Assert.IsNotNull(addedContent);
            Assert.AreEqual(6, addedContent.Id);
            Assert.AreEqual(25, addedContent.FileSize);
        }
    }
}
