using DocumentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.DocumentDALTests.Entities
{
    public class DocumentItemTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void DocumentMetadata_Filled()
        {
            //Arrange & Act
            DocumentItem item = new()
            {
                Id = 1,
                Title = "Test Title",
                Author = "Test Author",
                DocumentContent = null,
                DocumentMetadata = null,
                OcrText = "Example OCR Text string",
            };

            //Expected
            int expected_Id = 1;
            string expected_Title = "Test Title";
            string expected_Author = "Test Author";
            DocumentContent? expected_DocumentContent = null;
            DocumentMetadata? expected_DocumentMetadata = null;
            string? expected_OcrText = "Example OCR Text string";

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(expected_Id, Is.EqualTo(item.Id));
                Assert.That(expected_Title, Is.EqualTo(item.Title));
                Assert.That(expected_Author, Is.EqualTo(item.Author));
                Assert.That(expected_DocumentContent, Is.EqualTo(item.DocumentContent));
                Assert.That(expected_DocumentMetadata, Is.EqualTo(item.DocumentMetadata));
                Assert.That(expected_OcrText, Is.EqualTo(item.OcrText));
            });
        }
    }
}
