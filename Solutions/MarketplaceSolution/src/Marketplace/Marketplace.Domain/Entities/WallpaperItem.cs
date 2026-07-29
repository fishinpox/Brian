using Shared.Infrastructure.Common;

namespace Marketplace.Domain.Entities;

public class WallpaperItem : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public byte[] ImageData { get; private set; } = [];
    public string ContentType { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }

    private WallpaperItem() { }

    public static WallpaperItem Create(
        string name,
        string description,
        byte[] imageData,
        string contentType,
        string fileName,
        decimal price)
    {
        return new WallpaperItem
        {
            Name = name,
            Description = description,
            ImageData = imageData,
            ContentType = contentType,
            FileName = fileName,
            Price = price,
            IsActive = true
        };
    }
}
