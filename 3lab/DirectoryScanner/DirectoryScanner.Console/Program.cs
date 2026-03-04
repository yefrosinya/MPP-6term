using System.Threading;
using ScannerLib;

namespace DirectoryScanner.ConsoleApp;

public static class Program
{
    public static async Task Main(string[] args)
    {
        string path;
        if (args.Length > 0)
        {
            path = args[0];
        }
        else
        {
            System.Console.Write("Введите путь к каталогу: ");
            path = System.Console.ReadLine() ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            System.Console.WriteLine("Путь не задан.");
            return;
        }

        if (!Directory.Exists(path))
        {
            System.Console.WriteLine("Каталог не существует.");
            return;
        }

        var scanner = new ScannerCore(Environment.ProcessorCount);
        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            System.Console.WriteLine("Сканирование, нажмите Ctrl+C для отмены...");
            var root = await scanner.ScanAsync(path, cts.Token);
            PrintTree(root, 0);
        }
        catch (OperationCanceledException)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Сканирование отменено.");
        }
    }

    static void PrintTree(FileSystemNode node, int level)
    {
        var indent = new string(' ', level * 2);
        var icon = node.IsFolder ? "[D]" : "[F]";
        var percentText = level == 0 ? string.Empty : $" ({node.Percentage:F1}%)";
        System.Console.WriteLine($"{indent}{icon} {node.Name} - {node.Size} байт{percentText}");

        foreach (var child in node.Children.OrderByDescending(c => c.Size))
        {
            PrintTree(child, level + 1);
        }
    }
}

