using FluentValidation;

namespace Calendar.Application.Features.Backgrounds.Commands.SubmitCalendarBackground;

public class SubmitCalendarBackgroundCommandValidator : AbstractValidator<SubmitCalendarBackgroundCommand>
{
    private static readonly string[] AllowedContentTypes = ["image/png", "image/jpeg"];
    private const long MaxSizeBytes = 10 * 1024 * 1024;

    public SubmitCalendarBackgroundCommandValidator()
    {
        RuleFor(x => x.ImageData)
            .NotEmpty().WithMessage("Image data is required.")
            .Must(data => data.LongLength <= MaxSizeBytes).WithMessage("Image must be 10MB or smaller.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required.")
            .Must(ct => AllowedContentTypes.Contains(ct.ToLowerInvariant()))
            .WithMessage("Image must be PNG or JPEG.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(260);
    }
}
