namespace SkillSwap.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public int ExchangeId { get; set; }
        public int AuthorId { get; set; }

        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
