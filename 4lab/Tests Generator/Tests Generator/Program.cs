using TestsGeneratorLib;

if (args.Length < 3)
{
    Console.WriteLine("Usage: TestsGenerator <outputDir> <maxLoad> <maxGenerate> <maxWrite> <file1> [file2] ...");
    return;
}

var outputDir = args[0];
var maxLoad = int.Parse(args[1]);
var maxGenerate = int.Parse(args[2]);
var maxWrite = int.Parse(args[3]);
var files = args.Skip(4).ToList();

if (files.Count == 0)
{
    Console.WriteLine("No input files specified.");
    return;
}

if (!Directory.Exists(outputDir))
{
    Directory.CreateDirectory(outputDir);
}

var config = new PipelineConfig(maxLoad, maxGenerate, maxWrite);
var pipeline = new TestsGeneratorPipeline(config);
await pipeline.Generate(files, outputDir);

Console.WriteLine("Test generation completed.");
