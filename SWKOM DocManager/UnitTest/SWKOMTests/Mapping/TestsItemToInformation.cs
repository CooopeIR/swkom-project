using AutoMapper;
using DocumentDAL.Entities;
using SWKOM.Mappings;
using SWKOM.Models;

namespace UnitTest.SWKOMTests.Mapping
{
    public class TestsItemToInformation
    {
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        [Test]
        public void MappingProfile_CorrectFilled()
        {
            //Arrange
            DocumentItem item = new()
            {
                title = "TestTitle",
                author = "TestAuthor",
                id = 1,
                contentpath = "C://all",
            };

            //Expected from mapped
            DocumentInformation expected = new()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Id = 1,
                Contentpath = "C://all",
            };

            //Act
            var actual = _mapper.Map<DocumentItem, DocumentInformation>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Title, Is.EqualTo(expected.Title));
                Assert.That(actual.Author, Is.EqualTo(expected.Author));
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.Contentpath, Is.EqualTo(expected.Contentpath));
            });
        }

        [Test]
        public void MappingProfile_TitleEmpty()
        {
            //Arrange
            DocumentItem item = new()
            {
                author = "TestAuthor",
                id = 1,
                contentpath = "C://all",
            };

            //Expected from mapped
            DocumentInformation expected = new()
            {
                Title = "",
                Author = "TestAuthor",
                Id = 1,
                Contentpath = "C://all",
            };

            //Act
            var actual = _mapper.Map<DocumentItem, DocumentInformation>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Title, Is.EqualTo(expected.Title));
                Assert.That(actual.Author, Is.EqualTo(expected.Author));
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.Contentpath, Is.EqualTo(expected.Contentpath));
            });
        }

        [Test]
        public void MappingProfile_AuthorEmpty()
        {
            //Arrange
            DocumentItem item = new()
            {
                title = "TestTitle",
                id = 1,
                contentpath = "C://all",
            };

            //Expected from mapped
            DocumentInformation expected = new()
            {
                Title = "TestTitle",
                Author = "",
                Id = 1,
                Contentpath = "C://all",
            };

            //Act
            var actual = _mapper.Map<DocumentItem, DocumentInformation>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Title, Is.EqualTo(expected.Title));
                Assert.That(actual.Author, Is.EqualTo(expected.Author));
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.Contentpath, Is.EqualTo(expected.Contentpath));
            });
        }

        [Test]
        public void MappingProfile_ContentpathEmpty()
        {
            //Arrange
            DocumentItem item = new()
            {
                title = "TestTitle",
                author = "TestAuthor",
                id = 1,
            };

            //Expected from mapped
            DocumentInformation expected = new()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Id = 1,
                Contentpath = "",
            };

            //Act
            var actual = _mapper.Map<DocumentItem, DocumentInformation>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Title, Is.EqualTo(expected.Title));
                Assert.That(actual.Author, Is.EqualTo(expected.Author));
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.Contentpath, Is.EqualTo(expected.Contentpath));
            });
        }
    }
}