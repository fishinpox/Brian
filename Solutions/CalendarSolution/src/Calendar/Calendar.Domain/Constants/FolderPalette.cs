namespace Calendar.Domain.Constants;

public readonly record struct FolderColorTriplet(string Background, string Text, string Border);

public static class FolderPalette
{
    public static readonly IReadOnlyList<FolderColorTriplet> Swatches =
    [
        new("#E3F2FD", "#1976D2", "#2196F3"), // blue
        new("#F3E5F5", "#7B1FA2", "#9C27B0"), // purple
        new("#E8F5E9", "#388E3C", "#4CAF50"), // green
        new("#FFF3E0", "#EF6C00", "#FF9800"), // amber
        new("#E0F2F1", "#00796B", "#009688"), // teal
        new("#FCE4EC", "#C2185B", "#E91E63"), // pink
        new("#E8EAF6", "#3949AB", "#3F51B5"), // indigo
        new("#E0F7FA", "#00838F", "#00BCD4"), // cyan
        new("#FFEBEE", "#C62828", "#F44336"), // red
    ];

    public static FolderColorTriplet NextColor(int existingFolderCount) =>
        Swatches[existingFolderCount % Swatches.Count];
}
