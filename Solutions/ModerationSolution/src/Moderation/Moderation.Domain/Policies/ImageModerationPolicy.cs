namespace Moderation.Domain.Policies;

public static class ImageModerationPolicy
{
    public static readonly string[] AllowedContentTypes = ["image/png", "image/jpeg"];
    public const long MaxSizeBytes = 10 * 1024 * 1024;

    public static bool TryValidate(byte[] imageData, string contentType, out string? reason)
    {
        if (imageData.LongLength == 0)
        {
            reason = "Image is empty.";
            return false;
        }

        if (imageData.LongLength > MaxSizeBytes)
        {
            reason = "Image exceeds the 10MB size limit.";
            return false;
        }

        var normalizedContentType = contentType.ToLowerInvariant();
        if (!AllowedContentTypes.Contains(normalizedContentType))
        {
            reason = "Image must be PNG or JPEG.";
            return false;
        }

        if (!SignatureMatches(imageData, normalizedContentType))
        {
            reason = "Image content does not match its declared file type.";
            return false;
        }

        reason = null;
        return true;
    }

    private static bool SignatureMatches(byte[] data, string normalizedContentType) => normalizedContentType switch
    {
        "image/png" => data.Length >= 8
            && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47
            && data[4] == 0x0D && data[5] == 0x0A && data[6] == 0x1A && data[7] == 0x0A,
        "image/jpeg" => data.Length >= 3
            && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF,
        _ => false
    };
}
