namespace SkillSwap.Application.DTOs
{
    public class ChatConversationDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = null!;
        public string? PartnerAvatarUrl { get; set; }
        public string LastMessage { get; set; } = null!;
        public DateTime LastMessageAt { get; set; }
    }
}
