using Bubala.Application.Models;

namespace Bubala.Application.Models;

public class Product
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public string Slug => GenerateSlug();
    public required string Quantity { get; set; }
    public required List<string> FruitType { get; init; } = new();
    public string GenerateSlug()
    {
        return Name.ToLower().Replace(" ","-");
    }
}