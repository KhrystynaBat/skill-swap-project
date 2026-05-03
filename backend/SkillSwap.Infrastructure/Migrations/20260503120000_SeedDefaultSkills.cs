using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SkillSwap.Infrastructure.Persistence;

#nullable disable

namespace SkillSwap.Infrastructure.Migrations
{
    [DbContext(typeof(SkillSwapDbContext))]
    [Migration("20260503120000_SeedDefaultSkills")]
    public partial class SeedDefaultSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "Skills" ("Name", "Category")
                SELECT v."Name", v."Category"
                FROM (VALUES
                    ('Web Design', 'Design'),
                    ('UI/UX Design', 'Design'),
                    ('Photoshop', 'Design'),
                    ('Illustrator', 'Design'),
                    ('HTML', 'IT'),
                    ('CSS', 'IT'),
                    ('JavaScript', 'IT'),
                    ('TypeScript', 'IT'),
                    ('Angular', 'IT'),
                    ('React', 'IT'),
                    ('C#', 'IT'),
                    ('.NET', 'IT'),
                    ('English', 'Languages'),
                    ('German', 'Languages'),
                    ('Polish', 'Languages'),
                    ('Guitar', 'Music'),
                    ('Piano', 'Music'),
                    ('Singing', 'Music'),
                    ('Photography', 'Photography'),
                    ('Video Editing', 'Photography'),
                    ('Cooking', 'Lifestyle'),
                    ('Baking', 'Lifestyle'),
                    ('Math', 'Education'),
                    ('Statistics', 'Education'),
                    ('Public Speaking', 'Communication')
                ) AS v("Name", "Category")
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "Skills" s
                    WHERE s."Name" = v."Name"
                      AND s."Category" = v."Category"
                );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "Skills" s
                USING (VALUES
                    ('Web Design', 'Design'),
                    ('UI/UX Design', 'Design'),
                    ('Photoshop', 'Design'),
                    ('Illustrator', 'Design'),
                    ('HTML', 'IT'),
                    ('CSS', 'IT'),
                    ('JavaScript', 'IT'),
                    ('TypeScript', 'IT'),
                    ('Angular', 'IT'),
                    ('React', 'IT'),
                    ('C#', 'IT'),
                    ('.NET', 'IT'),
                    ('English', 'Languages'),
                    ('German', 'Languages'),
                    ('Polish', 'Languages'),
                    ('Guitar', 'Music'),
                    ('Piano', 'Music'),
                    ('Singing', 'Music'),
                    ('Photography', 'Photography'),
                    ('Video Editing', 'Photography'),
                    ('Cooking', 'Lifestyle'),
                    ('Baking', 'Lifestyle'),
                    ('Math', 'Education'),
                    ('Statistics', 'Education'),
                    ('Public Speaking', 'Communication')
                ) AS v("Name", "Category")
                WHERE s."Name" = v."Name"
                  AND s."Category" = v."Category"
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "UserSkills" us
                      WHERE us."SkillId" = s."Id"
                  )
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "UserInterests" ui
                      WHERE ui."SkillId" = s."Id"
                  );
                """);
        }
    }
}
