using FluentValidation;
using MongoDB.Bson;

namespace EMerx.DTOs.Reviews.Request;

public class ReviewRequestValidator : AbstractValidator<ReviewRequest>
{
    public ReviewRequestValidator()
    {
        RuleFor(x => x.UserId)
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("User ID must be a valid ObjectId");
        
        RuleFor(x => x.ProductId)
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("Product ID must be a valid ObjectId");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1.0, 5.0)
            .WithMessage("Rating must be between 1 and 5");
    }
}