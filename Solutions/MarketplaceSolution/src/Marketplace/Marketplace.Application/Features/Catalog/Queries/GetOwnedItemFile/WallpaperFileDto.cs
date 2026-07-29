namespace Marketplace.Application.Features.Catalog.Queries.GetOwnedItemFile;

public record WallpaperFileDto(byte[] ImageData, string ContentType, string FileName);
