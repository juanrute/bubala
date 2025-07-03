using Bubala.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bubala.Contracts.Requests;
using Bubala.Api.Auth;
using Bubala.Application.Models;
using Bubala.Api.Mapping;



namespace Bubala.Api.AddControllers;


[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [Authorize]
    [HttpPut(ApiEndpoints.Product.Review)]
    public async Task<IActionResult> ReviewMovie([FromRoute] Guid id,
        [FromBody] ReviewProductRequest reviewProduct,
        CancellationToken cancelationToken)
    {
        var userId = HttpContext.GetUserId();
        var result = await _reviewService.ReviewProductAsync(id, reviewProduct.review, userId!.Value, cancelationToken);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Product.DeleteReview)]
    public async Task<IActionResult> DeleteReview([FromRoute] Guid id,
        CancellationToken cancelationToken)
    {
        var userId = HttpContext.GetUserId();
        var result = await _reviewService.DeleteReviewAsinc(id, userId!.Value, cancelationToken);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Reviews.GetUserReviews)]
    public async Task<IActionResult> GetUserReviews(CancellationToken cancelationToken)
    {
        var userId = HttpContext.GetUserId();
        var reviews = await _reviewService.GetReviewsForUserAsync(userId!.Value, cancelationToken);
        var reviewResponse = reviews.MapToResponse();
        return  Ok(reviewResponse);
    }
}