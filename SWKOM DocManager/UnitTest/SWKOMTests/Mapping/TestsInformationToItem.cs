using AutoMapper;
using DocumentDAL.Entities;
using SWKOM.Mappings;
using SWKOM.Models;

namespace UnitTest.SWKOMTests.Mapping
{
    public class TestsInformationToItem
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
            DocumentInformation item = new()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Id = 1,
                Contentpath = "C://all",
            };

            //Expected from mapped
            DocumentItem expected = new()
            {
                title = "TestTitle",
                author = "TestAuthor",
                id = 1,
                contentpath = "C://all",
            };

            //Act
            var actual = _mapper.Map<DocumentInformation, DocumentItem>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.title, Is.EqualTo(expected.title));
                Assert.That(actual.author, Is.EqualTo(expected.author));
                Assert.That(actual.id, Is.EqualTo(expected.id));
                Assert.That(actual.contentpath, Is.EqualTo(expected.contentpath));
            });
        }


        [Test]
        public void MappingProfile_TitleEmpty()
        {
            //Arrange
            DocumentInformation item = new()
            {
                Author = "TestAuthor",
                Id = 1,
                Contentpath = "C://all",
            };

            //Expected from mapped
            DocumentItem expected = new()
            {
                title = "",
                author = "TestAuthor",
                id = 1,
                contentpath = "C://all",
            };

            //Act
            var actual = _mapper.Map<DocumentInformation, DocumentItem>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.title, Is.EqualTo(expected.title));
                Assert.That(actual.author, Is.EqualTo(expected.author));
                Assert.That(actual.id, Is.EqualTo(expected.id));
                Assert.That(actual.contentpath, Is.EqualTo(expected.contentpath));
            });
        }

        [Test]
        public void MappingProfile_AuthorEmpty()
        {
            //Arrange
            DocumentInformation item = new()
            {
                Title = "TestTitle",
                Id = 1,
                Contentpath = "C://all",
            };

            //Expected from mapped
            DocumentItem expected = new()
            {
                title = "TestTitle",
                author = "",
                id = 1,
                contentpath = "C://all",
            };

            //Act
            var actual = _mapper.Map<DocumentInformation, DocumentItem>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.title, Is.EqualTo(expected.title));
                Assert.That(actual.author, Is.EqualTo(expected.author));
                Assert.That(actual.id, Is.EqualTo(expected.id));
                Assert.That(actual.contentpath, Is.EqualTo(expected.contentpath));
            });
        }

        [Test]
        public void MappingProfile_ContentpathEmpty()
        {
            //ArrangeDocumentInformation item = new DocumentInformation()
            DocumentInformation item = new()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Id = 1,
            };

            //Expected from mapped
            DocumentItem expected = new()
            {
                title = "TestTitle",
                author = "TestAuthor",
                id = 1,
                contentpath = "",
            };

            //Act
            var actual = _mapper.Map<DocumentInformation, DocumentItem>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.title, Is.EqualTo(expected.title));
                Assert.That(actual.author, Is.EqualTo(expected.author));
                Assert.That(actual.id, Is.EqualTo(expected.id));
                Assert.That(actual.contentpath, Is.EqualTo(expected.contentpath));
            });
        }
    }
}