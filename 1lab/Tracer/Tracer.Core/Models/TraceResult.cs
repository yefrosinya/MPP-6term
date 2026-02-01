namespace Tracer.Core.Models;
using System.Collections.Generic;

public class TraceResult
{
    public IReadOnlyList<ThreadResult> Threads { get; }

    public TraceResult(List<ThreadResult> threads)
    {
        Threads = threads.AsReadOnly();
    }
}