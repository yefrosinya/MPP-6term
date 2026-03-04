using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ScannerLib;

namespace DirectoryScanner;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}

public class FolderToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isFolder)
            return isFolder ? "📁" : "📄";
        return "📄";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class BooleanToStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? "Сканирование..." : string.Empty;
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

