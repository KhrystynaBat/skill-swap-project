using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Hubs;
using SkillSwap.Application.DTOs;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Persistence;

namespace SkillSwap.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly SkillSwapDbContext _context;
        private readonly IHubContext<ChatHub> _chatHub;

        public ChatController(SkillSwapDbContext context, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _chatHub = chatHub;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var currentUserId = GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Unauthorized();

            var userMessages = await _context.ChatMessages
                .Where(m => m.SenderId == currentUserId.Value || m.ReceiverId == currentUserId.Value)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            var latestMessages = userMessages
                .GroupBy(m => GetPartnerId(m, currentUserId.Value))
                .Select(g => g.First())
                .ToList();

            var partnerIds = latestMessages
                .Select(m => GetPartnerId(m, currentUserId.Value))
                .Distinct()
                .ToList();

            var partners = await _context.Users
                .Where(u => partnerIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var conversations = latestMessages
                .Select(message =>
                {
                    var partnerId = GetPartnerId(message, currentUserId.Value);

                    if (!partners.TryGetValue(partnerId, out var partner))
                    {
                        return null;
                    }


                    return new ChatConversationDto
                    {
                        PartnerId = partner.Id,
                        PartnerName = partner.Name,
                        PartnerAvatarUrl = partner.AvatarUrl,
                        LastMessage = message.Text,
                        LastMessageAt = message.Timestamp
                    };
                })
                .Where(conversation => conversation != null)
                .ToList();

            return Ok(conversations);
        }

        [HttpGet("{partnerId:int}/messages")]
        public async Task<IActionResult> GetMessages(int partnerId)
        {
            var currentUserId = GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Unauthorized();

            var partnerExists = await _context.Users.AnyAsync(u => u.Id == partnerId);

            if (!partnerExists)
                return NotFound("User was not found");

            var messages = await _context.ChatMessages
                .Where(m =>
                    (m.SenderId == currentUserId.Value && m.ReceiverId == partnerId) ||
                    (m.SenderId == partnerId && m.ReceiverId == currentUserId.Value))
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Text = m.Text,
                    Timestamp = m.Timestamp,
                    IsMine = m.SenderId == currentUserId.Value
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("{receiverId:int}/messages")]
        public async Task<IActionResult> SendMessage(int receiverId, SendChatMessageDto dto)
        {
            var currentUserId = GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Unauthorized();

            var text = dto.Text?.Trim();

            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Message text is required");

            if (currentUserId.Value == receiverId)
                return BadRequest("Cannot send a message to yourself");

            var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);

            if (!receiverExists)
                return NotFound("Receiver was not found");

            var message = new ChatMessage
            {
                SenderId = currentUserId.Value,
                ReceiverId = receiverId,
                Text = text,
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            var senderMessage = ToDto(message, currentUserId.Value);
            var receiverMessage = ToDto(message, receiverId);

            await _chatHub.Clients.Group(GetUserGroupName(receiverId))
                .SendAsync("ReceiveMessage", receiverMessage);

            await _chatHub.Clients.Group(GetUserGroupName(currentUserId.Value))
                .SendAsync("ReceiveMessage", senderMessage);

            return Ok(senderMessage);
        }

        private int? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out var parsedUserId)
                ? parsedUserId
                : null;
        }

        private static string GetUserGroupName(int userId)
        {
            return $"user:{userId}";
        }

        private static int GetPartnerId(ChatMessage message, int currentUserId)
        {
            return message.SenderId == currentUserId
                ? message.ReceiverId
                : message.SenderId;
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
