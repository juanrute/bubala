namespace Bubala.Application.Models;

public class ProductReview
{
    public required Guid ProductId { get; init; }
    public required string slug { get; init; }
    public int Review { get; init; }
    
}