using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ScannerLib;

namespace DirectoryScanner;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}

public sealed class RelayCommand : ICommand
{
    readonly Action<object?> _execute;
    readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

public class MainViewModel : ViewModelBase
{
    FileSystemNode? _root;
    CancellationTokenSource? _cts;
    bool _isScanning;

    public FileSystemNode? Root
    {
        get => _root;
        set => SetField(ref _root, value);
    }

    public bool IsScanning
    {
        get => _isScanning;
        set => SetField(ref _isScanning, value);
    }

    public ICommand StartCommand => new RelayCommand(async _ => await StartScanAsync(), _ => !IsScanning);
    public ICommand CancelCommand => new RelayCommand(_ => _cts?.Cancel(), _ => IsScanning);

    async Task StartScanAsync()
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog();
        var result = dialog.ShowDialog();
        if (result != System.Windows.Forms.DialogResult.OK || string.IsNullOrWhiteSpace(dialog.SelectedPath))
            return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        IsScanning = true;

        var scanner = new ScannerCore(Environment.ProcessorCount);
        try
        {
            var token = _cts.Token;
            var root = await scanner.ScanAsync(dialog.SelectedPath, token).ConfigureAwait(false);
            Root = root;
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsScanning = false;
        }
    }
}