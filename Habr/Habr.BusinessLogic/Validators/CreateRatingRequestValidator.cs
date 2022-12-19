using FluentValidation;
using Habr.Common.Requests;
using Habr.Common.Resources;

namespace Habr.BusinessLogic.Validators
{
    public class CreateRatingRequestValidator : AbstractValidator<CreateRatingRequest>
    {
        public CreateRatingRequestValidator()
        {
            RuleFor(request => request.Value)
                .InclusiveBetween(1, 5)
                .WithMessage(ValidationMessageResource.InvalidRatingValue);
        }
    }
}