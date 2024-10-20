using FluentValidation.TestHelper;
using SWKOM.Models;
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
            var request = new DocumentInformation()
            {
                Title = "",
                Author = "TestAuthor",
            };

            //Act
            TestValidationResult<DocumentInformation> result = _validator.TestValidate(request);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Author);
        }

        [Test]
        public void DocumentItem_AuthorEmpty()
        {
            //Arrange
            var request = new DocumentInformation()
            {
                Title = "TestTitle",
                Author = "",
            };

            //Act
            TestValidationResult<DocumentInformation> result = _validator.TestValidate(request);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }

        [Test]
        public void DocumentItem_CorrectFilled()
        {
            //Arrange
            var request = new DocumentInformation()
            {
                Title = "TestTitle",
                Author = "TestAuthor",
            };

            //Act
            TestValidationResult<DocumentInformation> result = _validator.TestValidate(request);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Author);
        }

        [Test]
        public void DocumentItem_TitleTooLong()
        {
            //Arrange
            var request = new DocumentInformation()
            {
                Title = "Test_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_TitleTest_Title1",
                Author = "TestAuthor",
            };

            //Act
            TestValidationResult<DocumentInformation> result = _validator.TestValidate(request);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Author);
        }

        [Test]
        public void DocumentItem_AuthorTooLong()
        {
            //Arrange
            var request = new DocumentInformation()
            {
                Title = "TestTitle",
                Author = "TestAuthorTestAuthorTestAuthorTestAuthorTestAuthorTestAuthorTestAuthorTestAuthor1",
            };

            //Act
            TestValidationResult<DocumentInformation> result = _validator.TestValidate(request);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }
    }
}