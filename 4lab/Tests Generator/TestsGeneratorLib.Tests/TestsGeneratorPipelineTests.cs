using NUnit.Framework;
using TestsGeneratorLib;

namespace TestsGeneratorLib.Tests;

[TestFixture]
public class TestsGeneratorPipelineTests
{
    private string _inputDir = null!;
    private string _outputDir = null!;

    [SetUp]
    public void SetUp()
    {
        _inputDir = Path.Combine(Path.GetTempPath(), "TestGenInput_" + Guid.NewGuid());
        _outputDir = Path.Combine(Path.GetTempPath(), "TestGenOutput_" + Guid.NewGuid());
        Directory.CreateDirectory(_inputDir);
        Directory.CreateDirectory(_outputDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_inputDir))
            Directory.Delete(_inputDir, true);
        if (Directory.Exists(_outputDir))
            Directory.Delete(_outputDir, true);
    }

    [Test]
    public async Task Generate_SingleFile_CreatesTestFile()
    {
        var sourceFile = Path.Combine(_inputDir, "Input.cs");
        await File.WriteAllTextAsync(sourceFile, @"
namespace Sample
{
    public class Foo
    {
        public void Bar() { }
    }
}");

        var pipeline = new TestsGeneratorPipeline(new PipelineConfig(1, 1, 1));
        await pipeline.Generate(new[] { sourceFile }, _outputDir);

        Assert.That(File.Exists(Path.Combine(_outputDir, "FooTests.cs")), Is.True);
    }

    [Test]
    public async Task Generate_MultipleClassesInFile_CreatesSeparateFiles()
    {
        var sourceFile = Path.Combine(_inputDir, "Input.cs");
        await File.WriteAllTextAsync(sourceFile, @"
namespace Sample
{
    public class Alpha
    {
        public void Run() { }
    }

    public class Beta
    {
        public void Execute() { }
    }
}");

        var pipeline = new TestsGeneratorPipeline(new PipelineConfig(1, 1, 1));
        await pipeline.Generate(new[] { sourceFile }, _outputDir);

        Assert.That(File.Exists(Path.Combine(_outputDir, "AlphaTests.cs")), Is.True);
        Assert.That(File.Exists(Path.Combine(_outputDir, "BetaTests.cs")), Is.True);
    }

    [Test]
    public async Task Generate_MultipleFiles_ProcessesAll()
    {
        var file1 = Path.Combine(_inputDir, "File1.cs");
        var file2 = Path.Combine(_inputDir, "File2.cs");

        await File.WriteAllTextAsync(file1, @"
namespace Sample
{
    public class One { public void M() { } }
}");

        await File.WriteAllTextAsync(file2, @"
namespace Sample
{
    public class Two { public void N() { } }
}");

        var pipeline = new TestsGeneratorPipeline(new PipelineConfig(2, 2, 2));
        await pipeline.Generate(new[] { file1, file2 }, _outputDir);

        Assert.That(File.Exists(Path.Combine(_outputDir, "OneTests.cs")), Is.True);
        Assert.That(File.Exists(Path.Combine(_outputDir, "TwoTests.cs")), Is.True);
    }

    [Test]
    public async Task Generate_OutputFileContainsValidContent()
    {
        var sourceFile = Path.Combine(_inputDir, "Input.cs");
        await File.WriteAllTextAsync(sourceFile, @"
namespace Sample
{
    public class Widget
    {
        public int GetValue(int x) { return x; }
    }
}");

        var pipeline = new TestsGeneratorPipeline(new PipelineConfig(1, 1, 1));
        await pipeline.Generate(new[] { sourceFile }, _outputDir);

        var content = await File.ReadAllTextAsync(Path.Combine(_outputDir, "WidgetTests.cs"));
        Assert.That(content, Does.Contain("[TestFixture]"));
        Assert.That(content, Does.Contain("[Test]"));
        Assert.That(content, Does.Contain("GetValueTest"));
    }
}
