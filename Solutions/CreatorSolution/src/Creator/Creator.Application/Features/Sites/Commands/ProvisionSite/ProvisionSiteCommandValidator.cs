using Creator.Application.Common.Interfaces;
using Creator.Domain.Constants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Creator.Application.Features.Sites.Commands.ProvisionSite;

public class ProvisionSiteCommandValidator : AbstractValidator<ProvisionSiteCommand>
{
    public ProvisionSiteCommandValidator(ICreatorDbContext db)
    {
        RuleFor(x => x.RequestedSlug)
            .NotEmpty().WithMessage("A site address is required.")
            .Matches("^[a-z0-9](?:[a-z0-9-]{1,28}[a-z0-9])?$")
                .WithMessage("Use 3-30 lowercase letters, numbers, and hyphens, starting and ending with a letter or number.")
            .Must(slug => !ReservedSlugs.Values.Contains(slug))
                .WithMessage("That address is reserved. Please choose another.")
            .MustAsync(async (slug, ct) => !await db.Sites.AnyAsync(s => s.Slug == slug, ct))
                .WithMessage("That address is already taken.");
    }
}
