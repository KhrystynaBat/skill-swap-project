using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Persistence;
using Xunit;

namespace SkillSwap.Tests;

public class IntegrationTests
{
    [Fact]
    public async Task RegisterThenLogin_ReturnsJwtTokenOverHttp()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();

        var registerResponse = await app.Client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Integration User",
            email = "integration@test.com",
            password = "Password1"
        });

        var loginResponse = await app.Client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "integration@test.com",
            password = "Password1"
        });

        registerResponse.EnsureSuccessStatusCode();
        loginResponse.EnsureSuccessStatusCode();

        var loginJson = await JsonDocument.ParseAsync(
            await loginResponse.Content.ReadAsStreamAsync());

        Assert.False(string.IsNullOrWhiteSpace(loginJson.RootElement.GetProperty("token").GetString()));
    }

    [Fact]
    public async Task SearchUsers_FiltersByCategoryThroughAuthorizedHttpRequest()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();
        await app.SeedAsync(SeedSkillExchangeUsers);
        app.Client.AuthenticateAs(1, "olesia@test.com");

        var response = await app.Client.GetAsync("/api/users/search?category=Art");

        response.EnsureSuccessStatusCode();

        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var users = json.RootElement.EnumerateArray().ToList();

        Assert.Single(users);
        Assert.Equal(2, users[0].GetProperty("id").GetInt32());
        Assert.Equal("Khrystyna", users[0].GetProperty("name").GetString());
    }

    [Fact]
    public async Task MatchFlow_CreateAcceptAndList_UsesRealApiRequests()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();
        await app.SeedAsync(SeedSkillExchangeUsers);
        app.Client.AuthenticateAs(1, "olesia@test.com");

        var createResponse = await app.Client.PostAsync("/api/match/2", null);
        createResponse.EnsureSuccessStatusCode();

        var matchId = await app.ReadContextAsync(context =>
            context.Matches.Select(match => match.Id).SingleAsync());

        app.Client.AuthenticateAs(2, "khrystyna@test.com");
        var acceptResponse = await app.Client.PutAsync($"/api/match/{matchId}/status?status=active", null);
        var myMatchesResponse = await app.Client.GetAsync("/api/match/my");

        acceptResponse.EnsureSuccessStatusCode();
        myMatchesResponse.EnsureSuccessStatusCode();

        var json = await JsonDocument.ParseAsync(await myMatchesResponse.Content.ReadAsStreamAsync());
        var match = json.RootElement.EnumerateArray().Single();

        Assert.Equal(matchId, match.GetProperty("id").GetInt32());
        Assert.Equal("active", match.GetProperty("status").GetString());
    }

    [Fact]
    public async Task MatchRequestWithoutSharedInterests_ReturnsBadRequest()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();
        await app.SeedAsync(SeedSkillExchangeUsers);
        app.Client.AuthenticateAs(1, "olesia@test.com");

        var response = await app.Client.PostAsync("/api/match/3", null);
        var message = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Your skills and interests do not match", message);
    }

    [Fact]
    public async Task ReviewFlow_CompletesActiveMatchAndReturnsReviewForTargetUser()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();
        await app.SeedAsync(async context =>
        {
            await SeedSkillExchangeUsers(context);
            context.Matches.Add(new Match
            {
                Id = 10,
                UserAId = 1,
                UserBId = 2,
                Status = "active"
            });
        });
        app.Client.AuthenticateAs(1, "olesia@test.com");

        var createReviewResponse = await app.Client.PostAsJsonAsync("/api/review/user/2", new
        {
            rating = 5,
            comment = "Great job!"
        });

        app.Client.AuthenticateAs(2, "khrystyna@test.com");
        var reviewsResponse = await app.Client.GetAsync("/api/review/user/2");

        createReviewResponse.EnsureSuccessStatusCode();
        reviewsResponse.EnsureSuccessStatusCode();

        var status = await app.ReadContextAsync(context =>
            context.Matches.Where(match => match.Id == 10).Select(match => match.Status).SingleAsync());
        var json = await JsonDocument.ParseAsync(await reviewsResponse.Content.ReadAsStreamAsync());
        var review = json.RootElement.EnumerateArray().Single();

        Assert.Equal("completed", status);
        Assert.Equal(1, review.GetProperty("authorId").GetInt32());
        Assert.Equal(5, review.GetProperty("rating").GetInt32());
        Assert.Equal("Great job!", review.GetProperty("comment").GetString());
    }

    private static Task SeedSkillExchangeUsers(SkillSwapDbContext context)
    {
        context.Users.AddRange(
            new User { Id = 1, Name = "Olesia", Email = "olesia@test.com", PasswordHash = "hash", City = "Lviv" },
            new User { Id = 2, Name = "Khrystyna", Email = "khrystyna@test.com", PasswordHash = "hash", City = "Lviv" },
            new User { Id = 3, Name = "Vasyl", Email = "vasyl@test.com", PasswordHash = "hash", City = "Kyiv" });

        context.Skills.AddRange(
            new Skill { Id = 1, Name = "C#", Category = "IT" },
            new Skill { Id = 2, Name = "Photography", Category = "Art" },
            new Skill { Id = 3, Name = "Photoshop", Category = "Design" });

        context.UserSkills.AddRange(
            new UserSkill { Id = 1, UserId = 1, SkillId = 1, Level = 4 },
            new UserSkill { Id = 2, UserId = 2, SkillId = 2, Level = 5 },
            new UserSkill { Id = 3, UserId = 3, SkillId = 3, Level = 3 });

        context.UserInterests.AddRange(
            new UserInterest { Id = 1, UserId = 1, SkillId = 2, Priority = 5 },
            new UserInterest { Id = 2, UserId = 2, SkillId = 1, Priority = 4 },
            new UserInterest { Id = 3, UserId = 3, SkillId = 2, Priority = 3 });

        return Task.CompletedTask;
    }
}
