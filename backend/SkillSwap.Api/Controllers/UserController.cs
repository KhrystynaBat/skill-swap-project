namespace SkillSwap.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SkillSwap.Infrastructure.Persistence;

    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly SkillSwapDbContext _context;

        public UsersController(SkillSwapDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchBySkill(int skillId, string? city)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            var query = _context.UserSkills
                .Where(us => us.SkillId == skillId)
                .Include(us => us.User)
                .AsQueryable();

            query = query.Where(us => us.UserId != int.Parse(currentUserId));

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(us => us.User.City == city);
            }

            var users = await query
                .Select(us => new
                {
                    us.User.Id,
                    us.User.Name,
                    us.User.City,
                    us.Level
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize]
        [HttpGet("user/{userId}/rating")]
        public async Task<IActionResult> GetUserRating(int userId)
        {
            var ratings = await _context.Reviews
                .Where(r =>
                    _context.Exchanges.Any(e =>
                        e.Id == r.ExchangeId &&
                        (e.UserAId == userId || e.UserBId == userId)
                    ))
                .Select(r => r.Rating)
                .ToListAsync();

            if (!ratings.Any())
                return Ok(new { average = 0, count = 0 });

            var average = ratings.Average();

            return Ok(new
            {
                average = Math.Round(average, 2),
                count = ratings.Count
            });
        }


    }
}
