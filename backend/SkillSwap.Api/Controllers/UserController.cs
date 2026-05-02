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

        // [Authorize]
        // [HttpGet("search")]
        // public async Task<IActionResult> SearchBySkill(int skillId, string? city)
        // {
        //     var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        //     if (currentUserId == null)
        //         return Unauthorized();

        //     var query = _context.UserSkills
        //         .Where(us => us.SkillId == skillId)
        //         .Include(us => us.User)
        //         .AsQueryable();

        //     query = query.Where(us => us.UserId != int.Parse(currentUserId));

        //     if (!string.IsNullOrWhiteSpace(city))
        //     {
        //         query = query.Where(us => us.User.City == city);
        //     }

        //     var users = await query
        //         .Select(us => new
        //         {
        //             us.User.Id,
        //             us.User.Name,
        //             us.User.City,
        //             us.Level
        //         })
        //         .ToListAsync();

        //     return Ok(users);
        // }


        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(int? skillId, string? city, string? category)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            var query = _context.Users
                .Where(u => u.Id != userId)
                .AsQueryable();

            if (skillId.HasValue && skillId.Value > 0)
            {
                query = query.Where(u =>
                    _context.UserSkills.Any(us =>
                        us.UserId == u.Id && us.SkillId == skillId.Value));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(u =>
                    _context.UserSkills.Any(us =>
                        us.UserId == u.Id &&
                        us.Skill.Category == category));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var normalizedCity = city.Trim().ToLower();

                query = query.Where(u =>
                    u.City != null &&
                    u.City.ToLower().Contains(normalizedCity));
            }

            var users = await query
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.City,
                    u.AvatarUrl,

                    TeachSkills = _context.UserSkills
                        .Where(us => us.UserId == u.Id)
                        .Select(us => new
                        {
                            us.Skill.Name,
                            us.Skill.Category,
                            us.Level
                        })
                        .ToList(),

                    LearnSkills = _context.UserInterests
                        .Where(ui => ui.UserId == u.Id)
                        .Select(ui => new
                        {
                            ui.Skill.Name,
                            ui.Skill.Category,
                            ui.Priority
                        })
                        .ToList()
                })
                .Take(20)
                .ToListAsync();

            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.AvatarUrl,
                    u.Bio,
                    u.City,
                    u.Role,
                    u.CreatedAt,

                    TeachSkills = _context.UserSkills
                        .Where(us => us.UserId == u.Id)
                        .Select(us => new
                        {
                            us.Skill.Name,
                            us.Skill.Category,
                            us.Level
                        })
                        .ToList(),

                    LearnSkills = _context.UserInterests
                        .Where(ui => ui.UserId == u.Id)
                        .Select(ui => new
                        {
                            ui.Skill.Name,
                            ui.Skill.Category,
                            ui.Priority
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpGet("user/{userId}/rating")]
        public async Task<IActionResult> GetUserRating(int userId)
        {
            var ratings = await _context.Reviews
                .Where(r =>
                    _context.Matches.Any(m =>
                        m.Id == r.ExchangeId &&
                        (m.UserAId == userId || m.UserBId == userId) &&
                        r.AuthorId != userId
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
