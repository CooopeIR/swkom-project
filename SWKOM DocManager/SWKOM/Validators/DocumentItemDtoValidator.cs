using FluentValidation;
using SWKOM.DTO;

namespace SWKOM.Validators
{
    public class DocumentItemDtoValidator : AbstractValidator<DocumentItemDTO>
    {
        public DocumentItemDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(" Document Title must not be empty!")
                .MaximumLength(200).WithMessage(" Document Title must not exceed 200 characters!");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage(" Author must not be empty!")
                .MaximumLength(80).WithMessage(" Author must not exceed 80 characters!");

            RuleFor(x => x.UploadedFile)
                .NotNull().WithMessage("The UploadedFile field is required.");
        }
    }
}
