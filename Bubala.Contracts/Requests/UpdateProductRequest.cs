namespace Bubala.Contracts.Requests;

public class UpdateProductRequest
{
    public required string Name { get; init; }
    public required string Quantity { get; init; }
    public required IEnumerable<string> FruitType { get; init; } = Enumerable.Empty<string>();
 }