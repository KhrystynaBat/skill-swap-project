namespace SkillSwap.Domain.Entities
{
    public class Skill
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;

        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
        public ICollection<UserInterest> UserInterests { get; set; } = new List<UserInterest>();
    }

}
