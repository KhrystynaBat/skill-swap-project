using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Controllers;
using SkillSwap.Application.DTOs;
using SkillSwap.Domain.Entities;
using Xunit;

namespace SkillSwap.Tests;

public class ReviewControllerTests
{

    [Fact]
    public async Task CreateUserReview_ReturnsBadRequest_WhenReviewingSelf()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ReviewController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateUserReview(1, new CreateUserReviewDto { Rating = 5 });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Cannot review yourself", badRequest.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task CreateUserReview_ReturnsBadRequest_WhenRatingIsInvalid(int rating)
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ReviewController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateUserReview(2, new CreateUserReviewDto { Rating = rating });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Rating must be between 1 and 5", badRequest.Value);
    }

    [Fact]
    public async Task CreateUserReview_ReturnsBadRequest_WhenNoActiveMatchExists()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.Matches.Add(new Match { Id = 1, UserAId = 1, UserBId = 2, Status = "pending" });
        await context.SaveChangesAsync();
        var controller = new ReviewController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateUserReview(2, new CreateUserReviewDto { Rating = 5 });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("You can review only active matches", badRequest.Value);
    }

    [Fact]
    public async Task CreateUserReview_CreatesReviewAndCompletesMatch()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.Matches.Add(new Match { Id = 7, UserAId = 1, UserBId = 2, Status = "active" });
        await context.SaveChangesAsync();
        var controller = new ReviewController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateUserReview(2, new CreateUserReviewDto { Rating = 5, Comment = "Great" });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Review created", ok.Value);
        var review = await context.Reviews.SingleAsync();
        Assert.Equal(7, review.ExchangeId);
        Assert.Equal(1, review.AuthorId);
        Assert.Equal(5, review.Rating);
        Assert.Equal("completed", (await context.Matches.FindAsync(7))!.Status);
    }

    [Fact]
    public async Task CreateUserReview_ReturnsBadRequest_WhenAlreadyReviewed()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.Matches.Add(new Match { Id = 7, UserAId = 1, UserBId = 2, Status = "active" });
        context.Reviews.Add(new Review { ExchangeId = 7, AuthorId = 1, Rating = 5 });
        await context.SaveChangesAsync();
        var controller = new ReviewController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateUserReview(2, new CreateUserReviewDto { Rating = 4 });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("You already reviewed this user", badRequest.Value);
    }

    [Fact]
    public async Task GetUserReviews_ReturnsReviewsWrittenByOtherUsers()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.Matches.Add(new Match { Id = 7, UserAId = 1, UserBId = 2, Status = "completed" });
        context.Reviews.AddRange(
            new Review { Id = 1, ExchangeId = 7, AuthorId = 1, Rating = 5, Comment = "Good" },
            new Review { Id = 2, ExchangeId = 7, AuthorId = 2, Rating = 4, Comment = "Thanks" }
        );
        await context.SaveChangesAsync();
        var controller = new ReviewController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.GetUserReviews(2);

        var ok = Assert.IsType<OkObjectResult>(result);
        var rows = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
        Assert.Single(rows);
    }
}
