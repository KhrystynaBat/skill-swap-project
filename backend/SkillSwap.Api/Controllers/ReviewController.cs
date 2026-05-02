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
        [HttpPost("user/{targetUserId}")]
        public async Task<IActionResult> CreateUserReview(int targetUserId, CreateUserReviewDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            if (userId == targetUserId)
                return BadRequest("Cannot review yourself");

            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest("Rating must be between 1 and 5");

            var match = await _context.Matches
                .FirstOrDefaultAsync(m =>
                    m.Status == "active" &&
                    ((m.UserAId == userId && m.UserBId == targetUserId) ||
                     (m.UserAId == targetUserId && m.UserBId == userId)));

            if (match == null)
                return BadRequest("You can review only active matches");

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r =>
                    r.AuthorId == userId &&
                    r.ExchangeId == match.Id);

            if (alreadyReviewed)
                return BadRequest("You already reviewed this user");

            var review = new Review
            {
                ExchangeId = match.Id,
                AuthorId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.Reviews.Add(review);
            match.Status = "completed";
            await _context.SaveChangesAsync();

            return Ok("Review created");
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserReviews(int userId)
        {
            var reviews = await _context.Reviews
                .Where(r =>
                    _context.Matches.Any(m =>
                        m.Id == r.ExchangeId &&
                        (m.UserAId == userId || m.UserBId == userId) &&
                        r.AuthorId != userId
                    ))
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.AuthorId,
                    AuthorName = _context.Users
                        .Where(u => u.Id == r.AuthorId)
                        .Select(u => u.Name)
                        .FirstOrDefault(),
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }
    }
}
