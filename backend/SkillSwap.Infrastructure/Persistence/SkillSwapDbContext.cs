using Microsoft.EntityFrameworkCore;
using SkillSwap.Domain.Entities;

namespace SkillSwap.Infrastructure.Persistence
{
    public class SkillSwapDbContext : DbContext
    {
        public SkillSwapDbContext(DbContextOptions<SkillSwapDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<UserSkill> UserSkills => Set<UserSkill>();
        public DbSet<UserInterest> UserInterests => Set<UserInterest>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Exchange> Exchanges => Set<Exchange>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSkills)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.Skill)
                .WithMany(s => s.UserSkills)
                .HasForeignKey(us => us.SkillId);

            modelBuilder.Entity<UserInterest>()
                .HasOne(ui => ui.User)
                .WithMany(u => u.UserInterests)
                .HasForeignKey(ui => ui.UserId);

            modelBuilder.Entity<UserInterest>()
                .HasOne(ui => ui.Skill)
                .WithMany(s => s.UserInterests)
                .HasForeignKey(ui => ui.SkillId);
        }
    }

}
