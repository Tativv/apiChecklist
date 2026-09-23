using FluentValidation;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public sealed class UpdateChecklistTemplateRequestValidator : AbstractValidator<UpdateChecklistTemplateRequest>
{
    public UpdateChecklistTemplateRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Description).MaximumLength(1000);
        RuleFor(r => r.AreaId).NotEmpty();
        RuleFor(r => r.EstimatedDurationMinutes).GreaterThan(0);

        RuleFor(r => r.ExecutionMode)
            .NotEmpty()
            .Must(m => Enum.TryParse<TaskExecutionMode>(m, ignoreCase: true, out _))
            .WithMessage($"ExecutionMode must be one of: {string.Join(", ", Enum.GetNames<TaskExecutionMode>())}.");

        RuleFor(r => r.Schedules).NotEmpty().WithMessage("A template requires at least one schedule.");
        RuleForEach(r => r.Schedules).SetValidator(new ScheduleInputValidator());

        RuleFor(r => r.Tasks).NotEmpty();
        RuleForEach(r => r.Tasks).SetValidator(new ChecklistTaskRequestValidator());
    }
}
