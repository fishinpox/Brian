namespace Calendar.Application.Common.DTOs;

public record FolderedEventDto(Guid Id, string Title, DateTimeOffset StartAt, bool IsVisible, bool IsDraggable, bool IsCompleted);

public record SubfolderDto(Guid Id, Guid FolderId, string Name, bool IsVisible, bool IsLocked, List<FolderedEventDto> Events);

public record FolderDto(
    Guid Id,
    string Name,
    string ColorBackground,
    string ColorText,
    string ColorBorder,
    bool IsVisible,
    bool IsLocked,
    List<SubfolderDto> Subfolders);

public record FolderTreeDto(bool MasterLockEnabled, List<FolderDto> Folders);

public record CalendarSettingsDto(bool MasterLockEnabled, bool LockFoldersByDefault, bool AutoRecolorByTimeSensitivity, int DueSoonWindowHours);
