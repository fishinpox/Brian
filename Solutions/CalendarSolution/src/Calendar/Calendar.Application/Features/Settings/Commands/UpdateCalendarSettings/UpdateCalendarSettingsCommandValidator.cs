using FluentValidation;

namespace Calendar.Application.Features.Settings.Commands.UpdateCalendarSettings;

public class UpdateCalendarSettingsCommandValidator : AbstractValidator<UpdateCalendarSettingsCommand>
{
    public UpdateCalendarSettingsCommandValidator()
    {
        RuleFor(x => x.DueSoonWindowHours).InclusiveBetween(1, 24 * 30);
    }
}
