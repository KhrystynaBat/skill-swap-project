using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SkillSwap.Infrastructure.Persistence;

[ExcludeFromCodeCoverage]
public class SkillSwapDbContextFactory
    : IDesignTimeDbContextFactory<SkillSwapDbContext>
{
    public SkillSwapDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SkillSwapDbContext>();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new SkillSwapDbContext(optionsBuilder.Options);
    }
}
