using FluentValidation;
using Habr.Common.Requests;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;

namespace Habr.BusinessLogic.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator(RoleManager<Role> roleManager)
        {
            RuleFor(request => request.Name)
                .MaximumLength(200)
                .WithMessage(string.Format(ValidationMessageResource.FieldMustBeLessThanXSymbols, FieldNameResource.Name, 200))
                .When(request => request.Name != null);

            RuleFor(request => request.Email)
                .NotEmpty()
                .WithMessage(string.Format(ValidationMessageResource.FieldIsRequired, FieldNameResource.Email))
                .MaximumLength(200)
                .WithMessage(string.Format(ValidationMessageResource.FieldMustBeLessThanXSymbols, FieldNameResource.Email, 200))
                .EmailAddress()
                .WithMessage(request => string.Format(ValidationMessageResource.EmailIsInvalid, request.Email));

            RuleFor(request => request.Password)
                .NotEmpty()
                .WithMessage(string.Format(ValidationMessageResource.FieldIsRequired, FieldNameResource.Password))
                .MinimumLength(6)
                .WithMessage(string.Format(ValidationMessageResource.PasswordTooShort));

            RuleFor(request => request.Role)
                .Must(role => roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                .WithMessage(ValidationMessageResource.RoleIsIncorrect);
        }
    }
}