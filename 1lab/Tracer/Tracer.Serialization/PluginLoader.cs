using System.Reflection;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization;

public class PluginLoader
{
    public static List<ITraceResultSerializer> LoadSerializers(string pluginsDirectory)
    {
        var serializers = new List<ITraceResultSerializer>();
        
        if (!Directory.Exists(pluginsDirectory))
        {
            return serializers;
        }

        ResolveEventHandler resolveHandler = (sender, args) => // делегат для обработки событий, возникающих при сбое разрешения (поиска) сборки или ресурса
        {
            var assemblyName = new AssemblyName(args.Name);
            var dllPath = Path.Combine(pluginsDirectory, $"{assemblyName.Name}.dll");
            if (File.Exists(dllPath))
            {
                return Assembly.LoadFrom(dllPath);
            }
            return null;
        };
        
        AppDomain.CurrentDomain.AssemblyResolve += resolveHandler;

        try
        {
            var dllFiles = Directory.GetFiles(pluginsDirectory, "*.dll");
            
            foreach (var dllFile in dllFiles)
            {
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(dllFile);
                    var assembly = Assembly.Load(assemblyName);
                    var types = assembly.GetTypes()
                        .Where(t => typeof(ITraceResultSerializer).IsAssignableFrom(t) 
                                    && !t.IsInterface 
                                    && !t.IsAbstract);

                    foreach (var type in types)
                    {
                        if (Activator.CreateInstance(type) is ITraceResultSerializer serializer)
                        {
                            serializers.Add(serializer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось загрузить сборку {dllFile}: {ex.Message}");
                }
            }
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= resolveHandler;
        }

        return serializers;
    }
}
