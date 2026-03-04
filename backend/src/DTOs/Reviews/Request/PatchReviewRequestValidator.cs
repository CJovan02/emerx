using FluentValidation;

namespace EMerx.DTOs.Reviews.Request;

public class PatchReviewRequestValidator : AbstractValidator<PatchReviewRequest>
{
    public PatchReviewRequestValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1.0, 5.0)
            .When(x => x.Rating.HasValue)
            .WithMessage("Rating must be between 1 and 5");
    }
}
