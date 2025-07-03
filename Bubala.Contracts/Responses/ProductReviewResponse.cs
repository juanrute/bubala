namespace Bubala.Contracts.Responses;

public class ProductReviewResponse
{
    public required Guid ProductId { get; init; }
    public required string slug { get; init; }
    public int Review { get; init; }
    
}