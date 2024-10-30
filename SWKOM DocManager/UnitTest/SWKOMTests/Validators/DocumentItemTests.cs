using FluentValidation.TestHelper;
using SWKOM.DTO;
using SWKOM.Validators;

namespace UnitTest.SWKOMTests.Validators
{
    public class TestsDocumentItemValidator
    {
        private DocumentItemDtoValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new DocumentItemDtoValidator();
        }

        [Test]
        public void DocumentItem_TitleEmpty()
        {
            //Arrange
            var request = new DocumentItemDTO()
            {
                Title = "",
                Author = "TestAuthor",
            };

            //Act
            TestValidationResult<DocumentItemDTO> result = _validator.TestValidate(request);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Author);
        }

        [Test]
        public void DocumentItem_AuthorEmpty()
        {
            //Arrange
            var request = new DocumentItemDTO()
            {
                Title = "TestTitle",
                Author = "",
            };

            //Act
            TestValidationResult<DocumentItemDTO> result = _validator.TestValidate(request);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }

        [Test]
        public void DocumentItem_CorrectFilled()
        {
            //Arrange
            var request = new DocumentItemDTO()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
            };

            //Act
            TestValidationResult<DocumentItemDTO> result = _validator.TestValidate(request);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Author);
        }

        [Test]
        public void DocumentItem_TitleTooLong()
        {
            //Arrange
            var request = new DocumentItemDTO()
            {
                Title = "Test_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_Title1",
                Author = "TestAuthor",
            };

            //Act
            TestValidationResult<DocumentItemDTO> result = _validator.TestValidate(request);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Author);
        }

        [Test]
        public void DocumentItem_AuthorTooLong()
        {
            //Arrange
            var request = new DocumentItemDTO()
            {
                Title = "TestTitle",
                Author = "TestAuthorTestAuthorTestAuthorTestAuthorTestAuthorTestAuthorTestAuthorTestAuthor1",
            };

            //Act
            TestValidationResult<DocumentItemDTO> result = _validator.TestValidate(request);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }
    }
}