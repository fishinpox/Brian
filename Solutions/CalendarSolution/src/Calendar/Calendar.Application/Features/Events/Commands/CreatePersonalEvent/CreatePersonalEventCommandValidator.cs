using FluentValidation;

namespace Calendar.Application.Features.Events.Commands.CreatePersonalEvent;

public class CreatePersonalEventCommandValidator : AbstractValidator<CreatePersonalEventCommand>
{
    public CreatePersonalEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(x => x.StartAt)
            .NotEmpty().WithMessage("StartAt is required.");

        When(x => x.EndAt.HasValue, () =>
        {
            RuleFor(x => x.EndAt!.Value)
                .GreaterThan(x => x.StartAt)
                .WithMessage("EndAt must be after StartAt.");
        });
    }
}
