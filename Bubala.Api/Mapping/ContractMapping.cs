using Bubala.Contracts.Requests;
using Bubala.Contracts.Responses;
using Bubala.Application.Models;

namespace Bubala.Api.Mapping;

public static class ContractMapping
{
    public static Product MapToProduct(this CreateProductRequest createProductRequest)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = createProductRequest.Name,
            Quantity = createProductRequest.Quantity,
            FruitType = createProductRequest.FruitType.ToList()
        };
    }

    public static ProductResponse MapToProductResponse(this Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Quantity = product.Quantity,
            FruitType = product.FruitType.AsEnumerable(),
            Review = product.Review,
            UserReviews = product.UserReview
        };
    }

    public static Product MapToProduct(this UpdateProductRequest updateProductRequest, Guid id)
    {
        return new Product
        {
            Id = id,
            Name = updateProductRequest.Name,
            Quantity = updateProductRequest.Quantity,
            FruitType = updateProductRequest.FruitType.ToList()
        };
    }
    
    public static IEnumerable<ProductReviewResponse> MapToResponse(this IEnumerable<ProductReview> reviews)
    {
        return reviews.Select(x=> new ProductReviewResponse
        {
            ProductId = x.ProductId,
            slug = x.slug,
            Review = x.Review
        });
    }
}