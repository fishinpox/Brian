using FluentValidation;

namespace YouTube.Application.Features.YouTube.Commands.StoreYouTubeCredential;

public class StoreYouTubeCredentialCommandValidator : AbstractValidator<StoreYouTubeCredentialCommand>
{
    public StoreYouTubeCredentialCommandValidator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty().WithMessage("API key is required.");
    }
}
