using System.Text;
using Tracer.Core.Models;
using Tracer.Serialization.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tracer.Serialization.Yaml;

public class YamlTraceResultSerializer : ITraceResultSerializer
{
    public string Format => "yaml";

    public void Serialize(TraceResult traceResult, Stream to)
    {
        var yamlObject = new
        {
            threads = traceResult.Threads.Select(thread => new
            {
                id = thread.Id.ToString(),
                time = $"{thread.TotalTimeMs}ms",
                methods = ConvertMethods(thread.Methods)
            }).ToArray()
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(yamlObject);
        var bytes = Encoding.UTF8.GetBytes(yaml);
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
