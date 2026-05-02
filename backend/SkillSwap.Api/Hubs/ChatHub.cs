using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.DTOs;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Persistence;

namespace SkillSwap.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly SkillSwapDbContext _context;

        public ChatHub(SkillSwapDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();

            if (userId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId.Value));
            }

            await base.OnConnectedAsync();
        }

        public async Task<ChatMessageDto> SendMessage(int receiverId, string text)
        {
            var senderId = GetCurrentUserId();

            if (!senderId.HasValue)
                throw new HubException("Unauthorized");

            var normalizedText = text?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedText))
                throw new HubException("Message text is required");

            if (senderId.Value == receiverId)
                throw new HubException("Cannot send a message to yourself");

            var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);

            if (!receiverExists)
                throw new HubException("Receiver was not found");

            var message = new ChatMessage
            {
                SenderId = senderId.Value,
                ReceiverId = receiverId,
                Text = normalizedText,
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            var receiverMessage = ToDto(message, receiverId);
            var senderMessage = ToDto(message, senderId.Value);

            await Clients.Group(GetUserGroupName(receiverId))
                .SendAsync("ReceiveMessage", receiverMessage);

            await Clients.GroupExcept(
                    GetUserGroupName(senderId.Value),
                    new[] { Context.ConnectionId })
                .SendAsync("ReceiveMessage", senderMessage);

            return senderMessage;
        }

        private int? GetCurrentUserId()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out var parsedUserId)
                ? parsedUserId
                : null;
        }

        private static string GetUserGroupName(int userId)
        {
            return $"user:{userId}";
        }

        private static ChatMessageDto ToDto(ChatMessage message, int currentUserId)
        {
            return new ChatMessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Text = message.Text,
                Timestamp = message.Timestamp,
                IsMine = message.SenderId == currentUserId
            };
        }
    }
}
