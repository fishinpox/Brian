using FluentValidation;

namespace Calendar.Application.Features.Subfolders.Commands.CreateSubfolder;

public class CreateSubfolderCommandValidator : AbstractValidator<CreateSubfolderCommand>
{
    public CreateSubfolderCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
