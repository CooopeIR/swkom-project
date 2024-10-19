using FluentValidation.TestHelper;
using SWKOM.Models;
using SWKOM.Validators;

namespace UnitTest.SWKOMTests.Validators
{
    public class TestsDocumentItemValidator
    {
        [SetUp]
        public void Setup()
        {

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
            var validator = new DocumentItemDtoValidator();

            //Act
            TestValidationResult<DocumentInformation> result = validator.TestValidate(request);

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
            var validator = new DocumentItemDtoValidator();

            //Act
            TestValidationResult<DocumentInformation> result = validator.TestValidate(request);

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
            var validator = new DocumentItemDtoValidator();

            //Act
            TestValidationResult<DocumentInformation> result = validator.TestValidate(request);

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
            var validator = new DocumentItemDtoValidator();

            //Act
            TestValidationResult<DocumentInformation> result = validator.TestValidate(request);

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
            var validator = new DocumentItemDtoValidator();

            //Act
            TestValidationResult<DocumentInformation> result = validator.TestValidate(request);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }
    }
}