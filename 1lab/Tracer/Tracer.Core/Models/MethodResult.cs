namespace Tracer.Core.Models;
using System.Collections.Generic;

public class MethodResult
{
    public string MethodName { get; }
    public string ClassName { get; }
    public long TimeMs { get; }
    public IReadOnlyList<MethodResult> Methods { get; }

    public MethodResult(string methodName, string className, long timeMs, List<MethodResult> methods)
    {
        MethodName = methodName;
        ClassName = className;
        TimeMs = timeMs;
        Methods = methods.AsReadOnly();
    }
}