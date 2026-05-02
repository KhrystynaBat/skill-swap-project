using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Controllers;
using SkillSwap.Application.DTOs;
using SkillSwap.Domain.Entities;
using Xunit;

namespace SkillSwap.Tests;

public class ProfileControllerTests
{

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task AddSkill_ReturnsBadRequest_WhenLevelIsInvalid(int level)
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ProfileController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.AddSkill(new AddUserSkillDto { SkillId = 1, Level = level });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Level must be between 1 and 5", badRequest.Value);
    }

    [Fact]
    public async Task AddSkill_ReturnsNotFound_WhenSkillDoesNotExist()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ProfileController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.AddSkill(new AddUserSkillDto { SkillId = 99, Level = 3 });

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Skill not found", notFound.Value);
    }

    [Fact]
    public async Task AddSkill_AddsSkill()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ProfileController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.AddSkill(new AddUserSkillDto { SkillId = 1, Level = 4 });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Skill added", ok.Value);
        Assert.True(await context.UserSkills.AnyAsync(us => us.UserId == 1 && us.SkillId == 1));
    }

    [Fact]
    public async Task AddSkill_ReturnsBadRequest_WhenSkillAlreadyAdded()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.UserSkills.Add(new UserSkill { UserId = 1, SkillId = 1, Level = 3 });
        await context.SaveChangesAsync();
        var controller = new ProfileController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.AddSkill(new AddUserSkillDto { SkillId = 1, Level = 4 });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Skill already added", badRequest.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public async Task AddInterest_ReturnsBadRequest_WhenPriorityIsInvalid(int priority)
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ProfileController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.AddInterest(new AddUserInterestDto { SkillId = 1, Priority = priority });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Priority must be between 1 and 3", badRequest.Value);
    }

    [Fact]
    public async Task AddInterest_AddsInterest()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ProfileController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.AddInterest(new AddUserInterestDto { SkillId = 2, Priority = 3 });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Interest added", ok.Value);
        Assert.True(await context.UserInterests.AnyAsync(ui => ui.UserId == 1 && ui.SkillId == 2));
    }

    [Fact]
    public async Task GetMyProfile_ReturnsRatingFromMatchReviews()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.Matches.Add(new Match { Id = 8, UserAId = 1, UserBId = 2, Status = "completed" });
        context.Reviews.Add(new Review { ExchangeId = 8, AuthorId = 2, Rating = 4 });
        await context.SaveChangesAsync();
        var controller = new ProfileController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.GetMyProfile();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }
}
