using FluentValidation;

namespace Creator.Application.Features.Access.Commands.GrantAccess;

public class GrantAccessCommandValidator : AbstractValidator<GrantAccessCommand>
{
    public GrantAccessCommandValidator()
    {
        RuleFor(x => x.AccessLevel).IsInEnum().WithMessage("AccessLevel must be a valid SiteAccessLevel.");
    }
}
