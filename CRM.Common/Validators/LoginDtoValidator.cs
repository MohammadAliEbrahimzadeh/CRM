using CRM.Common.DTOs.Authentication;
using CRM.Common.Regex;
using FluentValidation;

namespace CRM.Common.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull();

    }
}
