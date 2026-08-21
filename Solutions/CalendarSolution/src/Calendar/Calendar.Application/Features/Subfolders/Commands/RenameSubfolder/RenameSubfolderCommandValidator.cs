using FluentValidation;

namespace Calendar.Application.Features.Subfolders.Commands.RenameSubfolder;

public class RenameSubfolderCommandValidator : AbstractValidator<RenameSubfolderCommand>
{
    public RenameSubfolderCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
