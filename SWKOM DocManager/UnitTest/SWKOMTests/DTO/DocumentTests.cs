using DocumentDAL.Entities;
using SWKOM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.SWKOMTests.DTO
{
    public class DocumentTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void DocumentMetadataDTO_Filled()
        {
            //Arrange & Act
            Document item = new()
            {
                Id = 1,
                Title = "Test title",
                Author = "Test author",
                OcrText = "This test document has content about test text.",
            };

            //Expected
            int expected_Id = 1;
            string expected_Title = "Test title";
            string expected_Author = "Test author";
            string expected_OcrText = "This test document has content about test text.";

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(expected_Id, Is.EqualTo(item.Id));
                Assert.That(expected_Title, Is.EqualTo(item.Title));
                Assert.That(expected_Author, Is.EqualTo(item.Author));
                Assert.That(expected_OcrText, Is.EqualTo(item.OcrText));
            });
        }
    }
}
