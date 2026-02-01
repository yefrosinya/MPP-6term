using System.Text;
using Newtonsoft.Json;
using Tracer.Core.Models;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Json;

public class JsonTraceResultSerializer : ITraceResultSerializer
{
    public string Format => "json";

    public void Serialize(TraceResult traceResult, Stream to)
    {
        var jsonObject = new
        {
            threads = traceResult.Threads.Select(thread => new
            {
                id = thread.Id.ToString(),
                time = $"{thread.TotalTimeMs}ms",
                methods = ConvertMethods(thread.Methods)
            }).ToArray()
        };

        var json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        var bytes = Encoding.UTF8.GetBytes(json);
        to.Write(bytes, 0, bytes.Length);
    }

    private object[] ConvertMethods(IReadOnlyList<MethodResult> methods)
    {
        return methods.Select(method => new
        {
            name = method.MethodName,
            @class = method.ClassName,
            time = $"{method.TimeMs}ms",
            methods = method.Methods.Count > 0 ? ConvertMethods(method.Methods) : null
        }).ToArray();
    }
}
