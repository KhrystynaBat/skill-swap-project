using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Persistence;

namespace SkillSwap.Tests;

internal static class TestHelpers
{
    public static SkillSwapDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SkillSwapDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SkillSwapDbContext(options);
    }

    public static void SetUser(ControllerBase controller, int userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, $"user{userId}@test.com")
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }

    public static async Task SeedUsersAndSkills(SkillSwapDbContext context)
    {
        context.Users.AddRange(
            new User { Id = 1, Name = "Olesia", Email = "olesia@test.com", PasswordHash = "hash" },
            new User { Id = 2, Name = "Khrystyna", Email = "khrystyna@test.com", PasswordHash = "hash" },
            new User { Id = 3, Name = "Vasyl", Email = "vasyl@test.com", PasswordHash = "hash" }
        );

        context.Skills.AddRange(
            new Skill { Id = 1, Name = "C#", Category = "IT" },
            new Skill { Id = 2, Name = "Photography", Category = "Art" },
            new Skill { Id = 3, Name = "Photoshop", Category = "Design" }
        );

        await context.SaveChangesAsync();
    }

    public static string? Text(IActionResult result)
    {
        return result switch
        {
            OkObjectResult ok => ok.Value?.ToString(),
            BadRequestObjectResult badRequest => badRequest.Value?.ToString(),
            NotFoundObjectResult notFound => notFound.Value?.ToString(),
            _ => null
        };
    }
}
