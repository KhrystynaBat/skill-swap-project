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
    [Route("api/exchange")]
    public class ExchangeController : ControllerBase
    {
        private readonly SkillSwapDbContext _context;

        public ExchangeController(SkillSwapDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateExchange(CreateExchangeDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == dto.MatchId);

            if (match == null)
                return NotFound("Match not found");

            if (match.Status != "active")
                return BadRequest("Match is not active");

            if (match.UserAId != userId && match.UserBId != userId)
                return Forbid();

            var exchange = new Exchange
            {
                UserAId = match.UserAId,
                UserBId = match.UserBId,
                ScheduledTime = dto.ScheduledTime,
                Status = "requested",
                RowVersion = BitConverter.GetBytes(DateTime.UtcNow.Ticks)
            };

            _context.Exchanges.Add(exchange);
            await _context.SaveChangesAsync();

            return Ok("Exchange created");
        }

        [Authorize]
        [HttpPut("{exchangeId}/confirm")]
        public async Task<IActionResult> ConfirmExchange(int exchangeId)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            var exchange = await _context.Exchanges
                .FirstOrDefaultAsync(e => e.Id == exchangeId);

            if (exchange == null)
                return NotFound();

            if (exchange.UserAId != userId && exchange.UserBId != userId)
                return Forbid();

            if (exchange.Status != "requested")
                return BadRequest("Exchange cannot be confirmed");

            exchange.Status = "confirmed";

            await _context.SaveChangesAsync();

            return Ok("Exchange confirmed");
        }

        [Authorize]
        [HttpPut("{exchangeId}/complete")]
        public async Task<IActionResult> CompleteExchange(int exchangeId)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            int userId = int.Parse(currentUserId);

            var exchange = await _context.Exchanges
                .FirstOrDefaultAsync(e => e.Id == exchangeId);

            if (exchange == null)
                return NotFound();

            if (exchange.UserAId != userId && exchange.UserBId != userId)
                return Forbid();

            if (exchange.Status != "confirmed")
                return BadRequest("Exchange must be confirmed first");

            exchange.Status = "completed";

            await _context.SaveChangesAsync();

            return Ok("Exchange completed");
        }
    }
}
