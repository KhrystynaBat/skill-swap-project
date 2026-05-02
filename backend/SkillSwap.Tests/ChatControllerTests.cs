using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Controllers;
using SkillSwap.Api.Hubs;
using SkillSwap.Application.DTOs;
using SkillSwap.Domain.Entities;
using Xunit;

namespace SkillSwap.Tests;

public class ChatControllerTests
{
    private static IHubContext<ChatHub> HubContext()
    {
        return new MockHubContext();
    }

    [Fact]
    public async Task GetMessages_ReturnsNotFound_WhenPartnerDoesNotExist()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ChatController(context, HubContext());
        TestHelpers.SetUser(controller, 1);

        var result = await controller.GetMessages(99);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User was not found", notFound.Value);
    }

    [Fact]
    public async Task GetMessages_ReturnsOnlyConversationMessages()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        context.ChatMessages.AddRange(
            new ChatMessage { SenderId = 1, ReceiverId = 2, Text = "Hi" },
            new ChatMessage { SenderId = 2, ReceiverId = 1, Text = "Hello" },
            new ChatMessage { SenderId = 3, ReceiverId = 2, Text = "Other" }
        );
        await context.SaveChangesAsync();
        var controller = new ChatController(context, HubContext());
        TestHelpers.SetUser(controller, 1);

        var result = await controller.GetMessages(2);

        var ok = Assert.IsType<OkObjectResult>(result);
        var messages = Assert.IsAssignableFrom<IEnumerable<ChatMessageDto>>(ok.Value);
        Assert.Equal(2, messages.Count());
    }

    [Fact]
    public async Task SendMessage_ReturnsBadRequest_WhenTextIsEmpty()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ChatController(context, HubContext());
        TestHelpers.SetUser(controller, 1);

        var result = await controller.SendMessage(2, new SendChatMessageDto { Text = "  " });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Message text is required", badRequest.Value);
    }

    [Fact]
    public async Task SendMessage_ReturnsBadRequest_WhenSendingToSelf()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ChatController(context, HubContext());
        TestHelpers.SetUser(controller, 1);

        var result = await controller.SendMessage(1, new SendChatMessageDto { Text = "Hi" });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Cannot send a message to yourself", badRequest.Value);
    }

    [Fact]
    public async Task SendMessage_SavesMessageAndReturnsDto()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedUsersAndSkills(context);
        var controller = new ChatController(context, HubContext());
        TestHelpers.SetUser(controller, 1);

        var result = await controller.SendMessage(2, new SendChatMessageDto { Text = "  Hello  " });

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ChatMessageDto>(ok.Value);
        Assert.Equal("Hello", dto.Text);
        Assert.True(dto.IsMine);
        Assert.True(await context.ChatMessages.AnyAsync(m => m.SenderId == 1 && m.ReceiverId == 2));
    }

    private sealed class MockHubContext : IHubContext<ChatHub>
    {
        public IHubClients Clients { get; } = new MockHubClients();
        public IGroupManager Groups { get; } = new MockGroupManager();
    }

    private sealed class MockHubClients : IHubClients
    {
        private readonly IClientProxy _proxy = new MockClientProxy();

        public IClientProxy All => _proxy;
        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => _proxy;
        public IClientProxy Client(string connectionId) => _proxy;
        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => _proxy;
        public IClientProxy Group(string groupName) => _proxy;
        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => _proxy;
        public IClientProxy Groups(IReadOnlyList<string> groupNames) => _proxy;
        public IClientProxy User(string userId) => _proxy;
        public IClientProxy Users(IReadOnlyList<string> userIds) => _proxy;
    }

    private sealed class MockClientProxy : IClientProxy
    {
        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class MockGroupManager : IGroupManager
    {
        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
