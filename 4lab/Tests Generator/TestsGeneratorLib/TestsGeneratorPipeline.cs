using System.Threading.Tasks.Dataflow;

namespace TestsGeneratorLib;

public class TestsGeneratorPipeline
{
    private readonly PipelineConfig _config;

    public TestsGeneratorPipeline(PipelineConfig config)
    {
        _config = config;
    }

    public Task Generate(IEnumerable<string> filePaths, string outputDirectory)
    {
        var loadBlock = new TransformBlock<string, string>(
            async path => await File.ReadAllTextAsync(path),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _config.MaxDegreeOfParallelismLoad
            });

        var generateBlock = new TransformManyBlock<string, TestClassInfo>(
            sourceCode => TestCodeGenerator.Generate(sourceCode),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _config.MaxDegreeOfParallelismGenerate
            });

        var writeBlock = new ActionBlock<TestClassInfo>(
            async testClass =>
            {
                var path = Path.Combine(outputDirectory, testClass.FileName);
                await File.WriteAllTextAsync(path, testClass.Content);
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _config.MaxDegreeOfParallelismWrite
            });

        var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
        loadBlock.LinkTo(generateBlock, linkOptions);
        generateBlock.LinkTo(writeBlock, linkOptions);

        foreach (var filePath in filePaths)
        {
            loadBlock.Post(filePath);
        }

        loadBlock.Complete();

        return writeBlock.Completion;
    }
}
