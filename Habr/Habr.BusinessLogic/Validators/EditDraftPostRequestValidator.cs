using FluentValidation;
using Habr.Common.Requests;
using Habr.Common.Resources;


namespace Habr.BusinessLogic.Validators
{
    public class EditDraftPostRequestValidator : AbstractValidator<EditDraftPostRequest>
    {
        public EditDraftPostRequestValidator()
        {
            RuleFor(request => request.NewTitle)
                .NotEmpty()
                .WithMessage(string.Format(ValidationMessageResource.FieldIsRequired, FieldNameResource.Title))
                .MaximumLength(200)
                .WithMessage(string.Format(ValidationMessageResource.FieldMustBeLessThanXSymbols, FieldNameResource.Title, 200));

            RuleFor(request => request.NewText)
                .NotEmpty()
                .WithMessage(string.Format(ValidationMessageResource.FieldIsRequired, FieldNameResource.Text))
                .MaximumLength(2000)
                .WithMessage(string.Format(ValidationMessageResource.FieldMustBeLessThanXSymbols, FieldNameResource.Text, 2000));
        }
    }
}