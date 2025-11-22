using FluentValidation;
using MongoDB.Bson;

namespace EMerx.DTOs.Id;

public class IdRequestValidator : AbstractValidator<IdRequest>
{
    public IdRequestValidator()
    {
        RuleFor(x => x.Id)
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("Id must be a valid ObjectId");
    }
}