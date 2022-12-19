using FluentValidation;
using Habr.Common;
using Habr.Common.Resources;

namespace Habr.BusinessLogic.Validators
{
    public class PaginationQueryParametersValidator : AbstractValidator<PaginationQueryParameters>
    {
        public PaginationQueryParametersValidator()
        {
            RuleFor(parameters => parameters.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage(ValidationMessageResource.InvalidPageSize);

            RuleFor(parameters => parameters.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage(ValidationMessageResource.InvalidPageNumber);
        }
    }
}