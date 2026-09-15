using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.Areas.Create;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.Areas;

public class CreateAreaHandlerTests
{
    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_WithNewName_ShouldCreateArea()
    {
        await using var db = CreateDbContext();
        var handler = new CreateAreaHandler(db);

        var result = await handler.Handle(new CreateAreaCommand("Lobby"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Lobby");
        (await db.Areas.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldReturnConflict()
    {
        await using var db = CreateDbContext();
        db.Areas.Add(new Area { Id = Guid.NewGuid(), Name = "Lobby" });
        await db.SaveChangesAsync();

        var handler = new CreateAreaHandler(db);

        var result = await handler.Handle(new CreateAreaCommand("Lobby"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }
}
