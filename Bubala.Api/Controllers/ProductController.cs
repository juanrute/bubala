using Microsoft.AspNetCore.Mvc;
using Bubala.Contracts.Requests;
using Bubala.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Bubala.Api.Auth;
using Bubala.Application.Services;

namespace Bubala.Api.AddControllers;


[ApiController]
public class ProductController : ControllerBase
{
    private IProductService _productService { get; set; }

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Product.Create)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest createProductRequest,
        CancellationToken cancelationToken)
    {
        var product = createProductRequest.MapToProduct();
        await _productService.CreateAsync(product,cancellationToken: cancelationToken);
        return CreatedAtAction(nameof(Get), new { idOrSlug = product.Id }, product);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Product.Get)]
    public async Task<IActionResult> Get(string idOrSlug,
        CancellationToken cancelationToken)
    {
        var userId = HttpContext.GetUserId();

        var product = await _productService.GetByIdOrSlugAsync(idOrSlug, userId, cancelationToken);
        
        if (product is null)
        {
            return NotFound();
        }
        return Ok(product.MapToProductResponse());
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Product.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancelationToken)
    {
        var userId = HttpContext.GetUserId();
        var products = await _productService.GetAllAsync(userId, cancelationToken);
        if (products is null)
        {
            return NotFound();
        }
        return Ok(products.Select(p => p.MapToProductResponse()));
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Product.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id,
        [FromBody] UpdateProductRequest updateProductRequest,
        CancellationToken cancelationToken)
    {
        var userId = HttpContext.GetUserId();
        var product = updateProductRequest.MapToProduct(id);
        var updatedProduct = await _productService.UpdateAsync(product, cancellationToken: cancelationToken);
        if (updatedProduct is null)
        {
            return NotFound();
        }
        var response = product.MapToProductResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Product.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id,
        CancellationToken cancelationToken)
    {
        var deleted = await _productService.DeleteByIdAsync(id, cancelationToken);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok();
    }
}