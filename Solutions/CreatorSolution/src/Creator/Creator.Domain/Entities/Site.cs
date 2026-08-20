using Creator.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Creator.Domain.Entities;

public class Site : BaseAuditableEntity
{
    public Guid OwnerProfileId { get; private set; }
    public string Slug { get; private set; } = string.Empty;
    public SitePublishState PublishState { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }

    private Site() { }

    public static Site Provision(Guid ownerProfileId, string slug)
    {
        return new Site
        {
            OwnerProfileId = ownerProfileId,
            Slug = slug,
            PublishState = SitePublishState.Draft,
            PublishedAt = null
        };
    }

    public void Publish()
    {
        PublishState = SitePublishState.Published;
        PublishedAt = DateTimeOffset.UtcNow;
    }

    public void Unpublish()
    {
        PublishState = SitePublishState.Draft;
        PublishedAt = null;
    }
}
