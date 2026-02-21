namespace SkillSwap.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SkillSwap.Application.DTOs;
    using SkillSwap.Domain.Entities;
    using SkillSwap.Infrastructure.Persistence;
    using System.Security.Claims;

    [ApiController]
    [Route("api/review")]
    public class ReviewController : ControllerBase
    {
        private readonly SkillSwapDbContext _context;

        public ReviewController(SkillSwapDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest("Rating must be between 1 and 5");

            var exchange = await _context.Exchanges
                .FirstOrDefaultAsync(e => e.Id == dto.ExchangeId);

            if (exchange == null)
                return NotFound("Exchange not found");

            if (exchange.Status != "completed")
                return BadRequest("Exchange must be completed");

            if (exchange.UserAId != userId && exchange.UserBId != userId)
                return Forbid();

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.ExchangeId == dto.ExchangeId && r.AuthorId == userId);

            if (alreadyReviewed)
                return BadRequest("You already reviewed this exchange");

            var review = new Review
            {
                ExchangeId = dto.ExchangeId,
                AuthorId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok("Review created");
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserReviews(int userId)
        {
            var reviews = await _context.Reviews
                .Where(r =>
                    _context.Exchanges.Any(e =>
                        e.Id == r.ExchangeId &&
                        (e.UserAId == userId || e.UserBId == userId)
                    ))
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }
    }
}
