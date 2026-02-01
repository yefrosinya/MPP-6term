using System.Collections.Concurrent;
using System.Diagnostics;
using Tracer.Core.Models;

namespace Tracer.Core;

public class Tracer : ITracer
{
    private readonly ConcurrentDictionary<int, ThreadTracer> _threads = new();

    public void StartTrace()
    {
        var stackTrace = new StackTrace();
        var frame = stackTrace.GetFrame(1);
        var method = frame.GetMethod();
        
        string methodName = method?.Name ?? "UnknownName";
        string className = method?.DeclaringType?.Name ?? "UnknownClass";
        int threadId = Environment.CurrentManagedThreadId;
        var threadTracer = _threads.GetOrAdd(threadId, _ => new ThreadTracer());
        var methodTracer = new MethodTracer(methodName, className);
        
        if (threadTracer.MethodStack.Count > 0)
        {
            var parent = threadTracer.MethodStack.Peek();
            parent.InternalMethods.Add(methodTracer);
        }
        else
        {
            threadTracer.RootMethods.Add(methodTracer);
        }

        threadTracer.MethodStack.Push(methodTracer);
        methodTracer.Stopwatch.Start();
    }
    
    public void StopTrace()
    {
        int threadId = Environment.CurrentManagedThreadId;
        if (_threads.TryGetValue(threadId, out var threadTracer))
        {
            if (threadTracer.MethodStack.Count > 0)
            {
                var method = threadTracer.MethodStack.Pop();
                method.Stopwatch.Stop();
            }
        }
    }

    public TraceResult GetTraceResult()
    {
        var threadResults = new List<ThreadResult>();
        foreach (var pair in _threads)
        {
            var methods = MapMethods(pair.Value.RootMethods);
            long totalTime = 0;
            foreach(var m in methods) totalTime += m.TimeMs;
            
            threadResults.Add(new ThreadResult(pair.Key, totalTime, methods));
        }
        
        return new TraceResult(threadResults);
    }
    
    private List<MethodResult> MapMethods(List<MethodTracer> tracers)
    {
        var results = new List<MethodResult>();
        foreach (var t in tracers)
        {
            results.Add(new MethodResult(
                t.MethodName, 
                t.ClassName, 
                t.Stopwatch.ElapsedMilliseconds, 
                MapMethods(t.InternalMethods)));
        }
        return results;
    }
}