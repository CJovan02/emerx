using FluentValidation;

namespace EMerx.DTOs.Products.Request;

public class PatchProductRequestValidator : AbstractValidator<PatchProductRequest>
{
    public PatchProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .Length(3, 30)
            .WithMessage("Product name must be between 5 and 30 characters long.");

        RuleFor(x => x.Category)
            .Length(3, 30)
            .WithMessage("Category name must be between 5 and 30 characters long.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");
    }
}