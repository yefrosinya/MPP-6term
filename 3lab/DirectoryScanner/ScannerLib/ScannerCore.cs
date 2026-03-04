using System.Collections.Concurrent;

namespace ScannerLib;

public class ScannerCore
{
    readonly int _maxThreads;

    public ScannerCore(int maxThreads = 4)
    {
        if (maxThreads <= 0) throw new ArgumentOutOfRangeException(nameof(maxThreads));
        _maxThreads = maxThreads;
    }

    public async Task<FileSystemNode> ScanAsync(string path, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path is empty.", nameof(path));

        var rootInfo = new DirectoryInfo(path);
        if (!rootInfo.Exists) throw new DirectoryNotFoundException(path);

        var rootNode = new FileSystemNode
        {
            Name = rootInfo.Name,
            FullPath = rootInfo.FullName,
            IsFolder = true
        };

        await ScanStructureAsync(rootNode, token).ConfigureAwait(false);
        RecalculateSizes(rootNode);
        CalculatePercentages(rootNode);
        return rootNode;
    }

    async Task ScanStructureAsync(FileSystemNode root, CancellationToken token)
    {
        var queue = new ConcurrentQueue<FileSystemNode>();
        queue.Enqueue(root);

        var workers = new List<Task>();
        for (var i = 0; i < _maxThreads; i++)
        {
            workers.Add(Task.Run(() => Worker(queue, token), token));
        }

        await Task.WhenAll(workers).ConfigureAwait(false);
    }

    void Worker(ConcurrentQueue<FileSystemNode> queue, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!queue.TryDequeue(out var node))
            {
                if (queue.IsEmpty)
                    break;
                continue;
            }

            try
            {
                if (string.IsNullOrEmpty(node.FullPath))
                    continue;

                var dirInfo = new DirectoryInfo(node.FullPath);
                if (!dirInfo.Exists)
                    continue;

                foreach (var file in dirInfo.EnumerateFiles())
                {
                    token.ThrowIfCancellationRequested();
                    if (file.LinkTarget != null) continue;

                    var fileNode = new FileSystemNode
                    {
                        Name = file.Name,
                        FullPath = file.FullName,
                        IsFolder = false,
                        Size = file.Length
                    };

                    node.Children.Add(fileNode);
                    node.Size += file.Length;
                }

                foreach (var dir in dirInfo.EnumerateDirectories())
                {
                    token.ThrowIfCancellationRequested();
                    if (dir.LinkTarget != null) continue;

                    var subDirNode = new FileSystemNode
                    {
                        Name = dir.Name,
                        FullPath = dir.FullName,
                        IsFolder = true
                    };

                    node.Children.Add(subDirNode);
                    queue.Enqueue(subDirNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (IOException)
            {
            }
        }
    }

    long RecalculateSizes(FileSystemNode node)
    {
        if (!node.IsFolder)
            return node.Size;

        long total = node.Size;
        foreach (var child in node.Children)
        {
            if (child.IsFolder)
                total += RecalculateSizes(child);
        }

        node.Size = total;
        return total;
    }

    void CalculatePercentages(FileSystemNode node)
    {
        foreach (var child in node.Children)
        {
            if (node.Size > 0)
                child.Percentage = (double)child.Size / node.Size * 100.0;

            if (child.IsFolder)
                CalculatePercentages(child);
        }
    }
}