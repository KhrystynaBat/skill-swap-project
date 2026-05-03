using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Api.Controllers;
using SkillSwap.Application.DTOs;
using SkillSwap.Domain.Entities;
using Xunit;

namespace SkillSwap.Tests;

public class ExchangeControllerTests
{
    [Fact]
    public async Task CreateExchange_ReturnsUnauthorized_WhenUserIsMissing()
    {
        await using var context = TestHelpers.CreateContext();
        var controller = new ExchangeController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await controller.CreateExchange(new CreateExchangeDto { MatchId = 1 });

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task CreateExchange_ReturnsNotFound_WhenMatchDoesNotExist()
    {
        await using var context = TestHelpers.CreateContext();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateExchange(new CreateExchangeDto { MatchId = 99 });

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Match not found", notFound.Value);
    }

    [Fact]
    public async Task CreateExchange_ReturnsBadRequest_WhenMatchIsNotActive()
    {
        await using var context = TestHelpers.CreateContext();
        context.Matches.Add(new Match { Id = 1, UserAId = 1, UserBId = 2, Status = "pending" });
        await context.SaveChangesAsync();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateExchange(new CreateExchangeDto { MatchId = 1 });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Match is not active", badRequest.Value);
    }

    [Fact]
    public async Task CreateExchange_ReturnsForbid_WhenUserIsNotMatchParticipant()
    {
        await using var context = TestHelpers.CreateContext();
        context.Matches.Add(new Match { Id = 1, UserAId = 2, UserBId = 3, Status = "active" });
        await context.SaveChangesAsync();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateExchange(new CreateExchangeDto { MatchId = 1 });

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task CreateExchange_CreatesRequestedExchange_WhenMatchIsActive()
    {
        await using var context = TestHelpers.CreateContext();
        var scheduledTime = DateTime.UtcNow.AddDays(1);
        context.Matches.Add(new Match { Id = 1, UserAId = 1, UserBId = 2, Status = "active" });
        await context.SaveChangesAsync();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CreateExchange(new CreateExchangeDto
        {
            MatchId = 1,
            ScheduledTime = scheduledTime
        });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Exchange created", ok.Value);
        var exchange = Assert.Single(context.Exchanges);
        Assert.Equal(1, exchange.UserAId);
        Assert.Equal(2, exchange.UserBId);
        Assert.Equal("requested", exchange.Status);
        Assert.Equal(scheduledTime, exchange.ScheduledTime);
        Assert.NotEmpty(exchange.RowVersion);
    }

    [Fact]
    public async Task ConfirmExchange_ReturnsNotFound_WhenExchangeDoesNotExist()
    {
        await using var context = TestHelpers.CreateContext();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.ConfirmExchange(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ConfirmExchange_ReturnsForbid_WhenUserIsNotParticipant()
    {
        await using var context = TestHelpers.CreateContext();
        context.Exchanges.Add(new Exchange
        {
            Id = 1,
            UserAId = 2,
            UserBId = 3,
            Status = "requested",
            RowVersion = [1]
        });
        await context.SaveChangesAsync();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.ConfirmExchange(1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task ConfirmExchange_ReturnsBadRequest_WhenStatusIsNotRequested()
    {
        await using var context = TestHelpers.CreateContext();
        context.Exchanges.Add(new Exchange
        {
            Id = 1,
            UserAId = 1,
            UserBId = 2,
            Status = "confirmed",
            RowVersion = [1]
        });
        await context.SaveChangesAsync();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.ConfirmExchange(1);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Exchange cannot be confirmed", badRequest.Value);
    }

    [Fact]
    public async Task ConfirmExchange_ChangesStatusToConfirmed()
    {
        await using var context = TestHelpers.CreateContext();
        context.Exchanges.Add(new Exchange
        {
            Id = 1,
            UserAId = 1,
            UserBId = 2,
            Status = "requested",
            RowVersion = [1]
        });
        await context.SaveChangesAsync();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 2);

        var result = await controller.ConfirmExchange(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Exchange confirmed", ok.Value);
        Assert.Equal("confirmed", context.Exchanges.Single().Status);
    }

    [Fact]
    public async Task CompleteExchange_ReturnsBadRequest_WhenExchangeIsNotConfirmed()
    {
        await using var context = TestHelpers.CreateContext();
        context.Exchanges.Add(new Exchange
        {
            Id = 1,
            UserAId = 1,
            UserBId = 2,
            Status = "requested",
            RowVersion = [1]
        });
        await context.SaveChangesAsync();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 1);

        var result = await controller.CompleteExchange(1);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Exchange must be confirmed first", badRequest.Value);
    }

    [Fact]
    public async Task CompleteExchange_ChangesStatusToCompleted()
    {
        await using var context = TestHelpers.CreateContext();
        context.Exchanges.Add(new Exchange
        {
            Id = 1,
            UserAId = 1,
            UserBId = 2,
            Status = "confirmed",
            RowVersion = [1]
        });
        await context.SaveChangesAsync();
        var controller = new ExchangeController(context);
        TestHelpers.SetUser(controller, 2);

        var result = await controller.CompleteExchange(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Exchange completed", ok.Value);
        Assert.Equal("completed", context.Exchanges.Single().Status);
    }
}
