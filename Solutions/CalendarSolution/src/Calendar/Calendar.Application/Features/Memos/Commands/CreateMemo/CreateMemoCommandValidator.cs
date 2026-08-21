using FluentValidation;

namespace Calendar.Application.Features.Memos.Commands.CreateMemo;

public class CreateMemoCommandValidator : AbstractValidator<CreateMemoCommand>
{
    public CreateMemoCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(500);
    }
}
