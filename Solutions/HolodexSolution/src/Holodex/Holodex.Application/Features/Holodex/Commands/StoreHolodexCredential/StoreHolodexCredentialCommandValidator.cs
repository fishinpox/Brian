using FluentValidation;

namespace Holodex.Application.Features.Holodex.Commands.StoreHolodexCredential;

public class StoreHolodexCredentialCommandValidator : AbstractValidator<StoreHolodexCredentialCommand>
{
    public StoreHolodexCredentialCommandValidator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty().WithMessage("API key is required.");
    }
}
