using Calendar.Domain.Enums;
using FluentValidation;

namespace Calendar.Application.Features.Reminders.Commands.CreateReminderPlan;

public class CreateReminderPlanCommandValidator : AbstractValidator<CreateReminderPlanCommand>
{
    public CreateReminderPlanCommandValidator()
    {
        RuleFor(x => x.MaxOccurrences).InclusiveBetween(1, 50);
        RuleFor(x => x.TimesOfDay).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.RecurrenceType != RecurrenceType.None || x.MaxOccurrences == 1)
            .WithMessage("A non-repeating reminder can only have 1 occurrence.");
        RuleFor(x => x)
            .Must(x => x.MaxOccurrences * x.TimesOfDay.Count <= 100)
            .WithMessage("A reminder plan can generate at most 100 individual reminders (occurrences x times-of-day).");
    }
}
