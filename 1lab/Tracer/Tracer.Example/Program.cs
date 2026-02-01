using Tracer.Core;
using Tracer.Core.Models;
using Tracer.Serialization;

var tracer = new Tracer.Core.Tracer();
var demo = new DemoScene(tracer);

demo.ExecuteMainLogic();
var thread = new Thread(() => {
    demo.SimpleMethod();
});
thread.Start();
thread.Join(); 

TraceResult result = tracer.GetTraceResult();

Console.WriteLine($"Записано потоков: {result.Threads.Count}");
foreach (var t in result.Threads)
{
    Console.WriteLine($"Поток #{t.Id}, время: {t.TotalTimeMs}ms");
}

var pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
if (!Directory.Exists(pluginsDirectory))
{
    pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
}

var serializers = PluginLoader.LoadSerializers(pluginsDirectory);
Console.WriteLine($"Загружено сериализаторов: {serializers.Count}");

foreach (var serializer in serializers)
{
    var fileName = $"result.{serializer.Format}";
    using (var fileStream = File.Create(fileName))
    {
        serializer.Serialize(result, fileStream);
    }
    Console.WriteLine($"Результат сохранен в {fileName}");
}

public class DemoScene
{
    private readonly ITracer _tracer;

    public DemoScene(ITracer tracer)
    {
        _tracer = tracer;
    }

    public void ExecuteMainLogic()
    {
        _tracer.StartTrace();
        Thread.Sleep(50);
        MethodWithInnerCall();
        _tracer.StopTrace();
    }

    private void MethodWithInnerCall()
    {
        _tracer.StartTrace();
        Thread.Sleep(100);
        _tracer.StopTrace();
    }

    public void SimpleMethod()
    {
        _tracer.StartTrace();
        Thread.Sleep(30);
        _tracer.StopTrace();
    }
}