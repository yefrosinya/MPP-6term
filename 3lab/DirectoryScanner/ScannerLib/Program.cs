using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ScannerLib;

public class FileSystemNode : INotifyPropertyChanged
{
    string _name = string.Empty;
    string _fullPath = string.Empty;
    bool _isFolder;
    long _size;
    double _percentage;

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public string FullPath
    {
        get => _fullPath;
        set
        {
            if (_fullPath == value) return;
            _fullPath = value;
            OnPropertyChanged();
        }
    }

    public bool IsFolder
    {
        get => _isFolder;
        set
        {
            if (_isFolder == value) return;
            _isFolder = value;
            OnPropertyChanged();
        }
    }

    public long Size
    {
        get => _size;
        set
        {
            if (_size == value) return;
            _size = value;
            OnPropertyChanged();
        }
    }

    public double Percentage
    {
        get => _percentage;
        set
        {
            if (Math.Abs(_percentage - value) < double.Epsilon) return;
            _percentage = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<FileSystemNode> Children { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void UpdateSize(long size)
    {
        Size = size;
    }
}