namespace SkillSwap.Application.DTOs
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public bool IsMine { get; set; }
    }
}
