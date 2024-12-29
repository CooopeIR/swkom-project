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
    public class DocumentItemRepositoryTests
    {
        private DocumentContext _context;
        private DocumentItemRepository _repository;

        [SetUp]
        public void SetUp()
        {
            // Create an in-memory database for testing purposes
            var options = new DbContextOptionsBuilder<DocumentContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DocumentContext(options);
            _repository = new DocumentItemRepository(_context);

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
        public async Task GetAllItemAsync_ShouldReturnAllDocumentItem()
        {
            // Act
            var result = await _repository.GetAllAsync();

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
        public async Task GetItemByIdAsyncReducedData_ShouldReturnDocumentItem_WhenIdExists()
        {
            // Act
            var result = await _repository.GetByIdAsync(1);

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
        public async Task GetItemByIdAsyncReducedData_ShouldThrowException_WhenIdDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetByIdAsync(99));
            Assert.That(ex.Message, Is.EqualTo("DocumentItem with ID 99 not found"));
        }

        [Test]
        public async Task AddItemAsync_ShouldAddNewDocumentItem()
        {
            // Arrange
            var newContent = new DocumentItem { Title = "Title Test 3", Author = "Author Test 3", OcrText = "This is example content." };

            // Act
            var result = await _repository.AddAsync(newContent);
            var addedContent = await _context.DocumentItems.FindAsync(3);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(addedContent, Is.Not.Null);
                Assert.That(addedContent.Title, Is.EqualTo("Title Test 3"));
                Assert.That(addedContent.Author, Is.EqualTo("Author Test 3"));
                Assert.That(addedContent.OcrText, Is.EqualTo("This is example content."));
            });
        }

        [Test]
        public async Task DeleteItemAsync_ShouldDeleteDocumentItem_WhenIdExists()
        {
            // Act
            await _repository.DeleteAsync(1);
            var deletedContent = await _context.DocumentContents.FindAsync(1);

            // Assert
            Assert.That(deletedContent, Is.Null);
        }

        [Test]
        public async Task DeleteItemAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Act
            await _repository.DeleteAsync(99);

            // Assert: No exception should be thrown
            Assert.Pass("Method should not throw an exception when trying to delete a non-existing record.");
        }

        [Test]
        public async Task UpdateItemAsync_ShouldNotThrowException_WhenIdDoesNotExist()
        {
            // Arrange
            var newContent = new DocumentItem { Title = "Title Test 3", Author = "Author Test 3", OcrText = "This is example content." };
            var result = await _repository.AddAsync(newContent);
            newContent.Title = "Another title";

            // Act
            await _repository.UpdateAsync(newContent);
            var addedContent = await _context.DocumentItems.FindAsync(3);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(addedContent, Is.Not.Null);
                Assert.That(result.Title, Is.EqualTo("Another title"));
                Assert.That(result.Author, Is.EqualTo("Author Test 3"));
                Assert.That(result.OcrText, Is.EqualTo("This is example content."));
            });
        }

        [Test]
        public async Task GetFullDocument_success()
        {
            // Arrange
            var newContent = new DocumentItem { Title = "Title Test 4", Author = "Author Test 4", OcrText = "This is example content.", DocumentContent = null, DocumentMetadata = null };
            var added = await _repository.AddAsync(newContent);

            // Act
            var result = await _repository.GetFullDocumentAsync(3);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Title, Is.EqualTo("Title Test 4"));
                Assert.That(result.Author, Is.EqualTo("Author Test 4"));
                Assert.That(result.OcrText, Is.EqualTo("This is example content."));
                Assert.That(result.DocumentContent, Is.Null);
                Assert.That(result.DocumentMetadata, Is.Null);
            });
        }

        [Test]
        public async Task GetFullDocument_notFound()
        {
            // Act
            var result = Assert.ThrowsAsync<Exception>(async () => await _repository.GetFullDocumentAsync(99));

            // Assert
            Assert.That(result.Message, Is.EqualTo("DocumentItem with ID 99 not found"));
        }
    }
}
