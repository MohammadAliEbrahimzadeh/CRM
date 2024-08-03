using CRM.Common.DTOs.Authentication;
using FluentValidation;

namespace CRM.Common.Validators;

public class SignInDtoValidator : AbstractValidator<SignInDto>
{
    public SignInDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Code)
            .GreaterThanOrEqualTo(100000).WithMessage("Code must be at least 6 digits.")
            .LessThanOrEqualTo(999999).WithMessage("Code must be at most 6 digits.");
    }
}
