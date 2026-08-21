using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class Folder : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ColorBackground { get; private set; } = string.Empty;
    public string ColorText { get; private set; } = string.Empty;
    public string ColorBorder { get; private set; } = string.Empty;
    public bool IsVisible { get; private set; } = true;
    public bool IsLocked { get; private set; }

    private Folder() { }

    public static Folder Create(Guid profileId, string name, string colorBackground, string colorText, string colorBorder, bool startLocked)
    {
        return new Folder
        {
            ProfileId = profileId,
            Name = name,
            ColorBackground = colorBackground,
            ColorText = colorText,
            ColorBorder = colorBorder,
            IsVisible = true,
            IsLocked = startLocked
        };
    }

    public void Rename(string name) => Name = name;

    public void SetColor(string background, string text, string border)
    {
        ColorBackground = background;
        ColorText = text;
        ColorBorder = border;
    }

    public void SetVisibility(bool isVisible) => IsVisible = isVisible;

    public void SetLock(bool isLocked) => IsLocked = isLocked;
}
