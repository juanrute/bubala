using Bubala.Application.Models;
using Bubala.Application.Repositories;
using FluentValidation;

namespace Bubala.Application.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    private readonly IProductRepository _productRepository;
    public ProductValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        RuleFor(p => p.Id)
            .NotEmpty();
        RuleFor(p => p.FruitType)
            .NotEmpty();
        RuleFor(p => p.Name)
            .NotEmpty();
        RuleFor(p => p.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This product already exists in the system");
    }

    private async Task<bool> ValidateSlug(Product product, string slug, CancellationToken cancelationToken = default)
    {
        var productExist = await _productRepository.GetBySlugAsync(slug);
        if (productExist is not null)
        {
            return productExist.Id == product.Id;
        }
        return productExist is null;
    }
}