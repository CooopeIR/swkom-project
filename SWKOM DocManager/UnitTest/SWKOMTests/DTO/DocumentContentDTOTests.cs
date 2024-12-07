using DocumentDAL.Entities;
using SWKOM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.SWKOMTests.DTO
{
    public class DocumentContentDTOTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void DocumentContentDTO_Filled()
        {
            //Arrange & Act
            DocumentContentDTO item = new()
            {
                Id = 1,
                DocumentId = 1,
                FileName = "Testname",
                ContentType = "Test ContentType",
                Content = new byte[1],
            };

            //Expected
            int expected_Id = 1;
            int expected_DocumentId = 1;
            string? expected_FileName = "Testname";
            string? expected_ContentType = "Test ContentType";
            byte[]? expected_Content = new byte[1];

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(expected_Id, Is.EqualTo(item.Id));
                Assert.That(expected_FileName, Is.EqualTo(item.FileName));
                Assert.That(expected_ContentType, Is.EqualTo(item.ContentType));
                Assert.That(expected_Content, Is.EqualTo(item.Content));
                Assert.That(expected_DocumentId, Is.EqualTo(item.DocumentId));
            });
        }
    }
}
