using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScannerLib;
using Xunit;

namespace ScannerLib.Tests;

public class ScannerCoreTests
{
    [Fact]
    public async Task ScanAsync_ComputesSizesForSimpleStructure()
    {
        var (rootPath, expectedTotal) = CreateTestDirectory();
        try
        {
            var scanner = new ScannerCore(4);
            var result = await scanner.ScanAsync(rootPath, CancellationToken.None);

            Assert.Equal(expectedTotal, result.Size);
        }
        finally
        {
            if (Directory.Exists(rootPath))
                Directory.Delete(rootPath, true);
        }
    }

    static (string rootPath, long expectedTotalSize) CreateTestDirectory()
    {
        var basePath = Path.Combine(Path.GetTempPath(), "ScannerLibTests");
        if (Directory.Exists(basePath))
            Directory.Delete(basePath, true);

        var root = Path.Combine(basePath, "root");
        Directory.CreateDirectory(root);

        var file1 = Path.Combine(root, "file1.txt");
        var file2 = Path.Combine(root, "file2.txt");
        var subDir = Path.Combine(root, "sub");
        Directory.CreateDirectory(subDir);
        var nestedFile = Path.Combine(subDir, "nested.txt");

        File.WriteAllText(file1, new string('a', 10), Encoding.UTF8);
        File.WriteAllText(file2, new string('b', 20), Encoding.UTF8);
        File.WriteAllText(nestedFile, new string('c', 30), Encoding.UTF8);

        var total = new FileInfo(file1).Length +
                    new FileInfo(file2).Length +
                    new FileInfo(nestedFile).Length;

        return (root, total);
    }
}

