using AutoMapper;
using DocumentDAL.Entities;
using SWKOM.Mappings;
using SWKOM.Models;

namespace UnitTest.SWKOMTests.Mapping
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void MappingProfile_DocumentItemToDocumentInformation()
        {
            //Arrange
            DocumentItem item = new DocumentItem()
            {
                title = "TestTitle",
                author = "TestAuthor",
                id = 1,
                contentpath = "C://all",
            };

            //Expected from mapped
            DocumentInformation expected = new DocumentInformation()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Id = 1,
                Contentpath = "C://all",
            };

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            //Act
            var actual = mapper.Map<DocumentItem, DocumentInformation>(item);

            //Assert
            Assert.NotNull(actual);
            Assert.That(actual.Title, Is.EqualTo(expected.Title));
            Assert.That(actual.Author, Is.EqualTo(expected.Author));
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Contentpath, Is.EqualTo(expected.Contentpath));
        }

        [Test]
        public void MappingProfile_DocumentInformationToDocumentItem()
        {
            //Arrange
            DocumentInformation item = new DocumentInformation()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Id = 1,
                Contentpath = "C://all",
            };

            //Expected from mapped
            DocumentItem expected = new DocumentItem()
            {
                title = "TestTitle",
                author = "TestAuthor",
                id = 1,
                contentpath = "C://all",
            };

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            //Act
            var actual = mapper.Map<DocumentInformation, DocumentItem>(item);

            //Assert
            Assert.NotNull(actual);
            Assert.That(actual.title, Is.EqualTo(expected.title));
            Assert.That(actual.author, Is.EqualTo(expected.author));
            Assert.That(actual.id, Is.EqualTo(expected.id));
            Assert.That(actual.contentpath, Is.EqualTo(expected.contentpath));
        }
    }
}