using System.Diagnostics;

namespace Tracer.Core;

internal class MethodTracer
{
    public string MethodName { get; set; }
    public string ClassName { get; set; }
    public Stopwatch Stopwatch { get; set; }
    public List<MethodTracer> InternalMethods { get; } = new();
    
    public MethodTracer(string methodName, string className)
    {
        MethodName = methodName;
        ClassName = className;
        Stopwatch = new Stopwatch();
    }
}

internal class ThreadTracer
{
    public List<MethodTracer> RootMethods { get; } = new();
    public Stack<MethodTracer> MethodStack { get; } = new();
}