using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistTemplates.List;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.ChecklistTemplates;

public class ListChecklistTemplatesHandlerTests
{
    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_ShouldExcludeSnapshots()
    {
        await using var db = CreateDbContext();
        var areaId = Guid.NewGuid();
        var groupId = Guid.NewGuid();

        db.ChecklistTemplates.AddRange(
            new ChecklistTemplate { Id = groupId, GroupId = groupId, IsSnapshot = true, Name = "Vieja", AreaId = areaId },
            new ChecklistTemplate { Id = Guid.NewGuid(), GroupId = groupId, IsSnapshot = false, Name = "Viva", AreaId = areaId });
        await db.SaveChangesAsync();

        var handler = new ListChecklistTemplatesHandler(db);
        var result = await handler.Handle(new ListChecklistTemplatesQuery(null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value[0].Name.Should().Be("Viva");
    }
}
