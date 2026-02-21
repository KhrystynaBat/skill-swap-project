namespace SkillSwap.Domain.Entities
{
    public class UserSkill
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int SkillId { get; set; }

        public int Level { get; set; }

        public User User { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}
