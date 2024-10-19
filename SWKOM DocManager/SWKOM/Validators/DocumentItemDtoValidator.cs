using FluentValidation;
using SWKOM.Models;

namespace SWKOM.Validators
{
    public class DocumentItemDtoValidator : AbstractValidator<DocumentInformation>
    {
        public DocumentItemDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(" Document Title must not be empty!")
                .MaximumLength(200).WithMessage(" Document Title must not exceed 200 characters!");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage(" Author must not be empty!")
                .MaximumLength(80).WithMessage(" Author must not exceed 80 characters!");
        }
    }
}
