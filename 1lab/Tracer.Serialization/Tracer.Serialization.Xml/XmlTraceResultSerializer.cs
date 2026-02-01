using System.Text;
using System.Xml.Linq;
using Tracer.Core.Models;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Xml;

public class XmlTraceResultSerializer : ITraceResultSerializer
{
    public string Format => "xml";

    public void Serialize(TraceResult traceResult, Stream to)
    {
        var root = new XElement("root",
            new XElement("threads",
                traceResult.Threads.Select(thread => new XElement("thread",
                    new XAttribute("id", thread.Id),
                    new XAttribute("time", $"{thread.TotalTimeMs}ms"),
                    ConvertMethods(thread.Methods)
                ))
            )
        );

        var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        var xml = document.ToString();
        var bytes = Encoding.UTF8.GetBytes(xml);
        to.Write(bytes, 0, bytes.Length);
    }

    private XElement[] ConvertMethods(IReadOnlyList<MethodResult> methods)
    {
        return methods.Select(method => new XElement("method",
            new XAttribute("name", method.MethodName),
            new XAttribute("class", method.ClassName),
            new XAttribute("time", $"{method.TimeMs}ms"),
            method.Methods.Count > 0 ? ConvertMethods(method.Methods) : null
        )).ToArray();
    }
}
