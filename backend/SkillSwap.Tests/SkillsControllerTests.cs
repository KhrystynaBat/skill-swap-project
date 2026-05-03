using Microsoft.AspNetCore.Mvc;
using SkillSwap.Api.Controllers;
using SkillSwap.Domain.Entities;
using Xunit;

namespace SkillSwap.Tests;

public class SkillsControllerTests
{
    [Fact]
    public async Task GetSkills_ReturnsSkillsOrderedByCategoryThenName()
    {
        await using var context = TestHelpers.CreateContext();
        context.Skills.AddRange(
            new Skill { Id = 1, Name = "Photoshop", Category = "Design" },
            new Skill { Id = 2, Name = "C#", Category = "IT" },
            new Skill { Id = 3, Name = "Figma", Category = "Design" });
        await context.SaveChangesAsync();
        var controller = new SkillsController(context);

        var result = await controller.GetSkills();

        var ok = Assert.IsType<OkObjectResult>(result);
        var rows = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value).ToList();
        Assert.Equal(3, rows.Count);
        Assert.Contains("Figma", rows[0].ToString());
        Assert.Contains("Photoshop", rows[1].ToString());
        Assert.Contains("C#", rows[2].ToString());
    }

    [Fact]
    public async Task GetSkills_ReturnsEmptyList_WhenNoSkillsExist()
    {
        await using var context = TestHelpers.CreateContext();
        var controller = new SkillsController(context);

        var result = await controller.GetSkills();

        var ok = Assert.IsType<OkObjectResult>(result);
        var rows = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
        Assert.Empty(rows);
    }
}
