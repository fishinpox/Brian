using FluentValidation;

namespace Calendar.Application.Features.Folders.Commands.RenameFolder;

public class RenameFolderCommandValidator : AbstractValidator<RenameFolderCommand>
{
    public RenameFolderCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
