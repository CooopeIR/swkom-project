using DocumentDAL.Entities;
using SWKOM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.SWKOMTests.DTO
{
    public class DocumentMetadataDTOTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void DocumentMetadataDTO_Filled()
        {
            //Arrange & Act
            DateTime now = DateTime.Now;
            DocumentMetadataDTO item = new()
            {
                Id = 1,
                DocumentId = 1,
                UploadDate = now,
                FileSize = 115,
            };

            //Expected
            int expected_Id = 1;
            int expected_DocumentId = 1;
            DateTime expected_UploadDate = now;
            int? expected_FileSize = 115;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(expected_Id, Is.EqualTo(item.Id));
                Assert.That(expected_UploadDate, Is.EqualTo(item.UploadDate));
                Assert.That(expected_FileSize, Is.EqualTo(item.FileSize));
                Assert.That(expected_DocumentId, Is.EqualTo(item.DocumentId));
            });
        }
    }
}
