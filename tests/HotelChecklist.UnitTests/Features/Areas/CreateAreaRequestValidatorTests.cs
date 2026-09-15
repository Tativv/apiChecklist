using FluentValidation.TestHelper;
using HotelChecklist.Api.Features.Areas.Create;

namespace HotelChecklist.UnitTests.Features.Areas;

public class CreateAreaRequestValidatorTests
{
    private readonly CreateAreaRequestValidator _validator = new();

    [Fact]
    public void EmptyName_ShouldHaveValidationError()
    {
        var result = _validator.TestValidate(new CreateAreaRequest(""));

        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public void NameTooLong_ShouldHaveValidationError()
    {
        var result = _validator.TestValidate(new CreateAreaRequest(new string('a', 201)));

        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public void ValidName_ShouldNotHaveValidationError()
    {
        var result = _validator.TestValidate(new CreateAreaRequest("Lobby"));

        result.ShouldNotHaveValidationErrorFor(r => r.Name);
    }
}
