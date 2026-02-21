namespace SkillSwap.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public string Role { get; set; } = "user";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
        public ICollection<UserInterest> UserInterests { get; set; } = new List<UserInterest>();
        public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
    }
}
