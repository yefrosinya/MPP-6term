namespace TestsGeneratorLib;

public class TestClassInfo
{
    public string FileName { get; }
    public string Content { get; }

    public TestClassInfo(string fileName, string content)
    {
        FileName = fileName;
        Content = content;
    }
}
