namespace TestsGeneratorLib;

public class PipelineConfig
{
    public int MaxDegreeOfParallelismLoad { get; }
    public int MaxDegreeOfParallelismGenerate { get; }
    public int MaxDegreeOfParallelismWrite { get; }

    public PipelineConfig(int maxLoad, int maxGenerate, int maxWrite)
    {
        MaxDegreeOfParallelismLoad = maxLoad;
        MaxDegreeOfParallelismGenerate = maxGenerate;
        MaxDegreeOfParallelismWrite = maxWrite;
    }
}
