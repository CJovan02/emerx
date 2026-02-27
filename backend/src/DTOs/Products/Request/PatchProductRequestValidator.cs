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

        RuleFor(x => x.Stock)
            .GreaterThan(0)
            .WithMessage("Stock must be greater than 0.");

        RuleFor(x => x.Description)
            .Length(10, 300)
            .WithMessage("Product description must be between 10 and 300 characters long.");
    }
}