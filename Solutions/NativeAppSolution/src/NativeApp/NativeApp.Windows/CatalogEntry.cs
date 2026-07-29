using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NativeApp.Windows;

public class CatalogEntry(Guid id, string name, string description) : INotifyPropertyChanged
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Description { get; } = description;

    private string _statusLabel = "Buy";
    public string StatusLabel
    {
        get => _statusLabel;
        set { _statusLabel = value; OnPropertyChanged(); }
    }

    private bool _canBuy = true;
    public bool CanBuy
    {
        get => _canBuy;
        set { _canBuy = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
