using FluentValidation;

namespace EMerx.DTOs.Email;

public class EmailRequestValidator : AbstractValidator<EmailRequest>
{
    public EmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email must be a valid email address");
    }
}