using CRM.Common.DTOs.Authentication;
using CRM.Common.Regex;
using FluentValidation;

namespace CRM.Common.Validators;

public class SendForgotPasswordEmailDtoValidator : AbstractValidator<SendForgotPasswordEmailDto>
{
    public SendForgotPasswordEmailDtoValidator()
    {

        RuleFor(x => x.Email)
            .Matches(RegexPatterns.Email)
            .WithMessage("Wrong Email Format");

    }
}
