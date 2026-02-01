namespace Tracer.Core.Models;
using System.Collections.Generic;

public class ThreadResult
{
    public int Id { get; }
    public long TotalTimeMs { get; }
    public IReadOnlyList<MethodResult> Methods { get; }
    
    public ThreadResult(int id, long totalTimeMs, List<MethodResult> methods)
    {
        Id = id;
        TotalTimeMs = totalTimeMs;
        Methods = methods.AsReadOnly();
    }
}