namespace SkillSwap.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SkillSwap.Infrastructure.Persistence;
    using SkillSwap.Domain.Entities;
    using System.Security.Claims;

    [ApiController]
    [Route("api/match")]
    public class MatchController : ControllerBase
    {
        private readonly SkillSwapDbContext _context;

        public MatchController(SkillSwapDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMatches()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            // Мої навички
            var mySkills = await _context.UserSkills
                .Where(us => us.UserId == userId)
                .Select(us => us.SkillId)
                .ToListAsync();

            // Мої інтереси
            var myInterests = await _context.UserInterests
                .Where(ui => ui.UserId == userId)
                .Select(ui => ui.SkillId)
                .ToListAsync();

            // Інші користувачі
            var matches = await _context.Users
                .Where(u => u.Id != userId)
                .Where(u =>
                    _context.UserSkills.Any(us =>
                        us.UserId == u.Id &&
                        myInterests.Contains(us.SkillId)) // вони вміють те, що я хочу
                    &&
                    _context.UserInterests.Any(ui =>
                        ui.UserId == u.Id &&
                        mySkills.Contains(ui.SkillId)) // вони хочуть те, що я вмію
                )
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.City
                })
                .ToListAsync();

            return Ok(matches);
        }

        [Authorize]
        [HttpPost("{targetUserId}")]
        public async Task<IActionResult> CreateMatch(int targetUserId)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            if (userId == targetUserId)
                return BadRequest("Cannot match with yourself");

            var targetExists = await _context.Users.AnyAsync(u => u.Id == targetUserId);

            if (!targetExists)
                return NotFound("User not found");

            var mySkills = await _context.UserSkills
                .Where(us => us.UserId == userId)
                .Select(us => us.SkillId)
                .ToListAsync();

            var myInterests = await _context.UserInterests
                .Where(ui => ui.UserId == userId)
                .Select(ui => ui.SkillId)
                .ToListAsync();

            var targetCanTeachWhatIWant = await _context.UserSkills
                .AnyAsync(us => us.UserId == targetUserId && myInterests.Contains(us.SkillId));

            var targetWantsWhatICanTeach = await _context.UserInterests
                .AnyAsync(ui => ui.UserId == targetUserId && mySkills.Contains(ui.SkillId));

            if (!targetCanTeachWhatIWant || !targetWantsWhatICanTeach)
                return BadRequest("Your skills and interests do not match");

            // Перевіряємо чи вже є match
            var exists = await _context.Matches
                .AnyAsync(m =>
                    (m.UserAId == userId && m.UserBId == targetUserId) ||
                    (m.UserAId == targetUserId && m.UserBId == userId));

            if (exists)
                return BadRequest("Match already exists");

            var match = new Match
            {
                UserAId = userId,
                UserBId = targetUserId,
                Status = "pending"
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return Ok("Match created");
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyMatches()
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            var matches = await _context.Matches
                .Where(m => m.UserAId == userId || m.UserBId == userId)
                .Select(m => new
                {
                    m.Id,
                    m.UserAId,
                    m.UserBId,
                    m.Status,
                    m.CreatedAt
                })
                .ToListAsync();

            return Ok(matches);
        }

        [Authorize]
        [HttpPut("{matchId}/status")]
        public async Task<IActionResult> UpdateMatchStatus(int matchId, string status)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                return NotFound();

            if (match.UserAId != userId && match.UserBId != userId)
                return Forbid();

            if (status != "active" && status != "rejected" && status != "completed")
                return BadRequest("Invalid status");

            match.Status = status;

            await _context.SaveChangesAsync();

            return Ok("Match updated");
        }
    }
}
