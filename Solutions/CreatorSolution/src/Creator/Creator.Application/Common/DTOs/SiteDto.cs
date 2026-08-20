using Creator.Domain.Enums;

namespace Creator.Application.Common.DTOs;

public record SiteDto(
    Guid Id,
    Guid OwnerProfileId,
    string Slug,
    SitePublishState PublishState,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    SiteAccessLevel MyAccessLevel);

public record PublicSiteDto(string Slug, SitePublishState PublishState);

public record SiteAccessGrantDto(
    Guid Id,
    Guid SiteId,
    Guid ProfileId,
    SiteAccessLevel AccessLevel,
    DateTimeOffset GrantedAt,
    Guid GrantedByProfileId);
