namespace Bubala.Contracts.Responses;

public class ProductResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required string Quantity { get; init; }
    public required IEnumerable<string> FruitType { get; init; } = Enumerable.Empty<string>();
 }