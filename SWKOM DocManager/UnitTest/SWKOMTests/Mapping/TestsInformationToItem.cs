using AutoMapper;
using DocumentDAL.Entities;
using SWKOM.DTO;
using SWKOM.Mappings;

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
            DocumentItemDTO item = new()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Id = 1,
            };

            //Expected from mapped
            DocumentItem expected = new()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Id = 1,
            };

            //Act
            var actual = _mapper.Map<DocumentItemDTO, DocumentItem>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Title, Is.EqualTo(expected.Title));
                Assert.That(actual.Author, Is.EqualTo(expected.Author));
            });
        }


        [Test]
        public void MappingProfile_TitleEmpty()
        {
            //Arrange
            DocumentItemDTO item = new()
            {
                Author = "TestAuthor",
                Id = 1,
            };

            //Expected from mapped
            DocumentItem expected = new()
            {
                Title = "",
                Author = "TestAuthor",
                Id = 1,
            };

            //Act
            var actual = _mapper.Map<DocumentItemDTO, DocumentItem>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Title, Is.EqualTo(expected.Title));
                Assert.That(actual.Author, Is.EqualTo(expected.Author));
            });
        }

        [Test]
        public void MappingProfile_AuthorEmpty()
        {
            //Arrange
            DocumentItemDTO item = new()
            {
                Title = "TestTitle",
                Id = 1,
            };

            //Expected from mapped
            DocumentItem expected = new()
            {
                Title = "TestTitle",
                Author = "",
                Id = 1,
            };

            //Act
            var actual = _mapper.Map<DocumentItemDTO, DocumentItem>(item);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Title, Is.EqualTo(expected.Title));
                Assert.That(actual.Author, Is.EqualTo(expected.Author));
            });
        }
    }
}