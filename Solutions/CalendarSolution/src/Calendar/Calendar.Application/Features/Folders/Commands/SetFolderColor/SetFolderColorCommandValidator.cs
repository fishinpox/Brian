using FluentValidation;

namespace Calendar.Application.Features.Folders.Commands.SetFolderColor;

public class SetFolderColorCommandValidator : AbstractValidator<SetFolderColorCommand>
{
    private const string HexPattern = "^#[0-9A-Fa-f]{6}$";

    public SetFolderColorCommandValidator()
    {
        RuleFor(x => x.Background).Matches(HexPattern).WithMessage("Background must be a hex color like #E3F2FD.");
        RuleFor(x => x.Text).Matches(HexPattern).WithMessage("Text must be a hex color like #1976D2.");
        RuleFor(x => x.Border).Matches(HexPattern).WithMessage("Border must be a hex color like #2196F3.");
    }
}
