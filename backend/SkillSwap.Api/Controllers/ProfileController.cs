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
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly SkillSwapDbContext _context;

        public ProfileController(SkillSwapDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.AvatarUrl,
                    u.Bio,
                    u.City,
                    u.Role,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            var ratings = await _context.Reviews
                .Where(r =>
                    _context.Matches.Any(m =>
                        m.Id == r.ExchangeId &&
                        (m.UserAId == userId || m.UserBId == userId) &&
                        r.AuthorId != userId
                    ))
                .Select(r => r.Rating)
                .ToListAsync();

            var average = ratings.Any() ? Math.Round(ratings.Average(), 2) : 0;

            return Ok(new
            {
                user,
                rating = new
                {
                    average,
                    count = ratings.Count
                }
            });
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(UpdateProfileDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null)
                return NotFound();

            if (dto.Name != null)
                user.Name = dto.Name;

            if (dto.Bio != null)
                user.Bio = dto.Bio;

            if (dto.City != null)
                user.City = dto.City;

            if (dto.AvatarUrl != null)
                user.AvatarUrl = dto.AvatarUrl;

            await _context.SaveChangesAsync();

            return Ok("Profile updated");
        }

        [Authorize]
        [HttpPost("skills")]
        public async Task<IActionResult> AddSkill(AddUserSkillDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            if (dto.Level < 1 || dto.Level > 5)
                return BadRequest("Level must be between 1 and 5");

            var skillExists = await _context.Skills
                .AnyAsync(s => s.Id == dto.SkillId);

            if (!skillExists)
                return NotFound("Skill not found");

            var alreadyExists = await _context.UserSkills
                .AnyAsync(us => us.UserId == int.Parse(userId) && us.SkillId == dto.SkillId);

            if (alreadyExists)
                return BadRequest("Skill already added");

            var userSkill = new UserSkill
            {
                UserId = int.Parse(userId),
                SkillId = dto.SkillId,
                Level = dto.Level
            };

            _context.UserSkills.Add(userSkill);
            await _context.SaveChangesAsync();

            return Ok("Skill added");
        }

        [Authorize]
        [HttpGet("skills")]
        public async Task<IActionResult> GetMySkills()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var skills = await _context.UserSkills
                .Where(us => us.UserId == int.Parse(userId))
                .Include(us => us.Skill)
                .Select(us => new
                {
                    us.Skill.Id,
                    us.Skill.Name,
                    us.Skill.Category,
                    us.Level
                })
                .ToListAsync();

            return Ok(skills);
        }

        [Authorize]
        [HttpPost("interests")]
        public async Task<IActionResult> AddInterest(AddUserInterestDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            if (dto.Priority < 1 || dto.Priority > 3)
                return BadRequest("Priority must be between 1 and 3");

            var skillExists = await _context.Skills
                .AnyAsync(s => s.Id == dto.SkillId);

            if (!skillExists)
                return NotFound("Skill not found");

            var alreadyExists = await _context.UserInterests
                .AnyAsync(ui => ui.UserId == int.Parse(userId) && ui.SkillId == dto.SkillId);

            if (alreadyExists)
                return BadRequest("Interest already added");

            var interest = new UserInterest
            {
                UserId = int.Parse(userId),
                SkillId = dto.SkillId,
                Priority = dto.Priority
            };

            _context.UserInterests.Add(interest);
            await _context.SaveChangesAsync();

            return Ok("Interest added");
        }

        [Authorize]
        [HttpGet("interests")]
        public async Task<IActionResult> GetMyInterests()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var interests = await _context.UserInterests
                .Where(ui => ui.UserId == int.Parse(userId))
                .Include(ui => ui.Skill)
                .Select(ui => new
                {
                    ui.Skill.Id,
                    ui.Skill.Name,
                    ui.Skill.Category,
                    ui.Priority
                })
                .ToListAsync();

            return Ok(interests);
        }
    }
}
