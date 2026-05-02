using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Controllers;
using SkillSwap.Domain.Entities;
using Xunit;

namespace SkillSwap.Tests;

public class MatchControllerTests
{

    [Fact]
    public async Task CreateMatch_ReturnsBadRequest_WhenMatchingWithSelf()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new MatchController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateMatch(1);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Cannot match with yourself", badRequest.Value);
    }

    [Fact]
    public async Task CreateMatch_ReturnsNotFound_WhenTargetUserDoesNotExist()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new MatchController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateMatch(99);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", notFound.Value);
    }

    [Fact]
    public async Task CreateMatch_ReturnsBadRequest_WhenSkillsAndInterestsDoNotMatch()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.UserSkills.Add(new UserSkill { UserId = 1, SkillId = 1, Level = 4 });
        context.UserInterests.Add(new UserInterest { UserId = 1, SkillId = 2, Priority = 3 });
        context.UserSkills.Add(new UserSkill { UserId = 2, SkillId = 3, Level = 4 });
        context.UserInterests.Add(new UserInterest { UserId = 2, SkillId = 1, Priority = 3 });
        await context.SaveChangesAsync();
        var controller = new MatchController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateMatch(2);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Your skills and interests do not match", badRequest.Value);
    }

    [Fact]
    public async Task CreateMatch_CreatesPendingMatch_WhenInterestsMatch()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.UserSkills.Add(new UserSkill { UserId = 1, SkillId = 1, Level = 4 });
        context.UserInterests.Add(new UserInterest { UserId = 1, SkillId = 2, Priority = 3 });
        context.UserSkills.Add(new UserSkill { UserId = 2, SkillId = 2, Level = 4 });
        context.UserInterests.Add(new UserInterest { UserId = 2, SkillId = 1, Priority = 3 });
        await context.SaveChangesAsync();
        var controller = new MatchController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateMatch(2);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Match created", ok.Value);
        var match = await context.Matches.SingleAsync();
        Assert.Equal(1, match.UserAId);
        Assert.Equal(2, match.UserBId);
        Assert.Equal("pending", match.Status);
    }

    [Fact]
    public async Task CreateMatch_ReturnsBadRequest_WhenMatchAlreadyExists()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.UserSkills.Add(new UserSkill { UserId = 1, SkillId = 1, Level = 4 });
        context.UserInterests.Add(new UserInterest { UserId = 1, SkillId = 2, Priority = 3 });
        context.UserSkills.Add(new UserSkill { UserId = 2, SkillId = 2, Level = 4 });
        context.UserInterests.Add(new UserInterest { UserId = 2, SkillId = 1, Priority = 3 });
        context.Matches.Add(new Match { UserAId = 2, UserBId = 1, Status = "pending" });
        await context.SaveChangesAsync();
        var controller = new MatchController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateMatch(2);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Match already exists", badRequest.Value);
    }

    [Fact]
    public async Task GetMyMatches_ReturnsOnlyCurrentUserMatches()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.Matches.AddRange(
            new Match { Id = 1, UserAId = 1, UserBId = 2, Status = "pending" },
            new Match { Id = 2, UserAId = 2, UserBId = 3, Status = "active" }
        );
        await context.SaveChangesAsync();
        var controller = new MatchController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.GetMyMatches();

        var ok = Assert.IsType<OkObjectResult>(result);
        var rows = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
        Assert.Single(rows);
    }

    [Theory]
    [InlineData("active")]
    [InlineData("rejected")]
    [InlineData("completed")]
    public async Task UpdateMatchStatus_AllowsSupportedStatuses(string status)
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.Matches.Add(new Match { Id = 1, UserAId = 1, UserBId = 2, Status = "pending" });
        await context.SaveChangesAsync();
        var controller = new MatchController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.UpdateMatchStatus(1, status);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(status, (await context.Matches.FindAsync(1))!.Status);
    }

    [Fact]
    public async Task UpdateMatchStatus_ReturnsBadRequest_ForUnsupportedStatus()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.Matches.Add(new Match { Id = 1, UserAId = 1, UserBId = 2, Status = "pending" });
        await context.SaveChangesAsync();
        var controller = new MatchController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.UpdateMatchStatus(1, "paused");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid status", badRequest.Value);
    }

    [Fact]
    public async Task UpdateMatchStatus_ReturnsForbid_WhenUserIsNotParticipant()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.Matches.Add(new Match { Id = 1, UserAId = 2, UserBId = 3, Status = "pending" });
        await context.SaveChangesAsync();
        var controller = new MatchController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.UpdateMatchStatus(1, "active");

        Assert.IsType<ForbidResult>(result);
    }
}
