using FluentValidation;

namespace Agency.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Category must be a valid CompanyCategory.");
    }
}
