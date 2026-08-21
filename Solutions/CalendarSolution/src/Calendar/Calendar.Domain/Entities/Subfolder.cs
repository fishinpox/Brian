using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class Subfolder : BaseAuditableEntity
{
    public Guid FolderId { get; private set; }
    public Guid ProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public bool IsVisible { get; private set; } = true;
    public bool IsLocked { get; private set; }

    private Subfolder() { }

    public static Subfolder Create(Guid folderId, Guid profileId, string name, bool startLocked)
    {
        return new Subfolder
        {
            FolderId = folderId,
            ProfileId = profileId,
            Name = name,
            IsVisible = true,
            IsLocked = startLocked
        };
    }

    public void Rename(string name) => Name = name;

    public void SetVisibility(bool isVisible) => IsVisible = isVisible;

    public void SetLock(bool isLocked) => IsLocked = isLocked;
}
