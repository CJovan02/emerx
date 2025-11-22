using FluentValidation;

namespace EMerx.DTOs.Products.Request;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .Length(5, 30)
            .WithMessage("Product name must be between 5 and 30 characters long.");

        RuleFor(x => x.Category)
            .Length(5, 30)
            .WithMessage("Category name must be between 5 and 30 characters long.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");
    }
}