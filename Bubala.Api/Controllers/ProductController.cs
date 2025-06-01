using Microsoft.AspNetCore.Mvc;
using Bubala.Application.Repositories;
using Bubala.Application.Models;
using Bubala.Contracts.Requests;
using Bubala.Api.Mapping;
using Bubala.Api;


namespace Bubala.Api.AddControllers;

[ApiController]
public class ProductController : ControllerBase
{
    private IProductRepository _productRepository { get; set; }

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpPost(ApiEndpoints.Product.Create)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest createProductRequest)
    {
        var product = createProductRequest.MapToProduct();
        await _productRepository.CreateAsync(product);
        return CreatedAtAction(nameof(Get), new { idOrSlug = product.Id }, product);
    }

    [HttpGet(ApiEndpoints.Product.Get)]
    public async Task<IActionResult> Get(string idOrSlug)
    {
        var product = Guid.TryParse(idOrSlug, out Guid id) ?
            await _productRepository.GetByIdAsync(id) :
            await _productRepository.GetBySlugAsync(idOrSlug);
        
        if (product is null)
        {
            return NotFound();
        }
        return Ok(product.MapToProductResponse());
    }

    [HttpGet(ApiEndpoints.Product.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        if (products is null)
        {
            return NotFound();
        }
        return Ok(products.Select(p => p.MapToProductResponse()));
    }

    [HttpPut(ApiEndpoints.Product.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductRequest updateProductRequest)
    {
        var product = updateProductRequest.MapToProduct(id);
        var updatded = await _productRepository.UpdateAsync(product);
        if (!updatded)
        {
            return NotFound();
        }
        var response = product.MapToProductResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Product.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _productRepository.DeleteByIdAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok();
    }
}