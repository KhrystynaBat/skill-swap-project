namespace SkillSwap.Domain.Entities
{
    public class Match
    {
        public int Id { get; set; }

        public int UserAId { get; set; }
        public int UserBId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "pending";
    }
}
