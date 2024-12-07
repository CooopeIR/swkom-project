using DocumentDAL.Entities;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWKOM.BusinessLogic;
using SWKOM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.SWKOMTests.BusinessLogic
{
    public class DocumentProcessorTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase(ExpectedResult = 1)]
        //public void DocumentProcessor_Filled()
        public async Task<int> DocumentProcessor_Filled()
        {
            //Arrange
            DocumentProcessor _documentProcessor = new();
            var contentBytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile dummyFile = new FormFile(new MemoryStream(contentBytes), 0, contentBytes.Length, "Data", "dummyFile.txt") { Headers = new HeaderDictionary(), ContentType = "Data" };
            DocumentItemDTO documentItemDTO = new()
            {
                Title = "Dummy",
                Author = "Dummy",
                Id = null,
                DocumentContentDto = null,
                DocumentMetadataDto = null,
                OcrText = null,

                UploadedFile = dummyFile,
            };

            //Expected
            string expected_Title = "Dummy";
            string expected_Author = "Dummy";
            DocumentContentDTO? expected_DocumentContentDto = new()
            {
                Content = contentBytes,
                FileName = "dummyFile.txt",
                ContentType = "Data",
            };
            DocumentMetadataDTO? expected_DocumentMetadataDto = new()
            {
                FileSize = contentBytes.Length,
                UploadDate = DateTime.Today.Date,
            };

            //Act
            documentItemDTO = await _documentProcessor.ProcessDocument(documentItemDTO);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(expected_Title, Is.EqualTo(documentItemDTO.Title));
                Assert.That(expected_Author, Is.EqualTo(documentItemDTO.Author));
                Assert.That(documentItemDTO.Id, Is.Null);
                Assert.That(documentItemDTO.DocumentContentDto, Is.Not.Null);
                if (documentItemDTO.DocumentContentDto != null)
                {
                    Assert.That(expected_DocumentContentDto.Content, Is.EqualTo(documentItemDTO.DocumentContentDto.Content));
                    Assert.That(expected_DocumentContentDto.FileName, Is.EqualTo(documentItemDTO.DocumentContentDto.FileName));
                    Assert.That(expected_DocumentContentDto.ContentType, Is.EqualTo(documentItemDTO.DocumentContentDto.ContentType));
                }
                Assert.That(documentItemDTO.DocumentMetadataDto, Is.Not.Null);
                if (documentItemDTO.DocumentMetadataDto != null)
                {
                    Assert.That(expected_DocumentMetadataDto.FileSize, Is.EqualTo(documentItemDTO.DocumentMetadataDto.FileSize));
                    Assert.That(expected_DocumentMetadataDto.UploadDate, Is.EqualTo(documentItemDTO.DocumentMetadataDto.UploadDate));
                    Assert.That(documentItemDTO.OcrText, Is.Null);
                }
            });
            return 1;
        }
    }
}
