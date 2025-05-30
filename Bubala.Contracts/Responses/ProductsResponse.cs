namespace Bubala.Contracts.Responses;

public class ProductsResponse
{
    public IEnumerable<ProductResponse> Items { get; init; } = Enumerable.Empty<ProductResponse>();
}