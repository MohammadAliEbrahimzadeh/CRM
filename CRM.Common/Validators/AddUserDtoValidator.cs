using CRM.Common.DTOs.Authentication;
using CRM.Common.Regex;
using FluentValidation;

namespace CRM.Common.Validators;

public class AddUserDtoValidator : AbstractValidator<AddUserDto>
{
    public AddUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Email)
            .Matches(RegexPatterns.Email)
            .WithMessage("Wrong Email Format");

        RuleFor(x => x.Password)
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must be at least 8 characters long, contain at least one uppercase letter," +
            " one lowercase letter, one digit, and one special character.");

    }
}
