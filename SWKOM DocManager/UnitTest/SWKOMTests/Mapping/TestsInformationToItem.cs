using AutoMapper;
using DocumentDAL.Entities;
using SWKOM.Mappings;
using SWKOM.Models;

namespace UnitTest.SWKOMTests.Mapping
{
    public class TestsInformationToItem
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void MappingProfile_CorrectFilled()
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


        [Test]
        public void MappingProfile_TitleEmpty()
        {
            //Arrange
            DocumentInformation item = new DocumentInformation()
            {
                Author = "TestAuthor",
                Id = 1,
                Contentpath = "C://all",
            };

            //Expected from mapped
            DocumentItem expected = new DocumentItem()
            {
                title = "",
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

        [Test]
        public void MappingProfile_AuthorEmpty()
        {
            //Arrange
            DocumentInformation item = new DocumentInformation()
            {
                Title = "TestTitle",
                Id = 1,
                Contentpath = "C://all",
            };

            //Expected from mapped
            DocumentItem expected = new DocumentItem()
            {
                title = "TestTitle",
                author = "",
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

        [Test]
        public void MappingProfile_ContentpathEmpty()
        {
            //ArrangeDocumentInformation item = new DocumentInformation()
            DocumentInformation item = new DocumentInformation()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Id = 1,
            };

            //Expected from mapped
            DocumentItem expected = new DocumentItem()
            {
                title = "TestTitle",
                author = "TestAuthor",
                id = 1,
                contentpath = "",
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