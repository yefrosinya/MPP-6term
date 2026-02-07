using System.Collections.Concurrent;
using System.Reflection;
using Faker.Generators;

namespace Faker;

public class Faker
{
    private readonly List<IValueGenerator> _generators = new();
    private readonly FakerConf _config;
    private readonly Random _random = new();
    private readonly HashSet<Type> _activeTypes = new();

    public Faker(FakerConf config = null)
    {
        _config = config ?? new FakerConf();
        _generators.Add(new IntGenerator());
        _generators.Add(new StringGenerator());
        _generators.Add(new ListGenerator());
    }

    public T Create<T>() => (T)Create(typeof(T));

    public object Create(Type type, string memberName = null, Type parentType = null)
    {
        if (parentType != null && memberName != null)
        {
            var customGenType = _config.GetGeneratorType(parentType, memberName);
            if (customGenType != null)
            {
                var gen = (IValueGenerator)Activator.CreateInstance(customGenType);
                return gen.Generate(type, new GeneratorContext(_random, this));
            }
        }

        var generator = _generators.FirstOrDefault(g => g.CanGenerate(type));
        if (generator != null)
            return generator.Generate(type, new GeneratorContext(_random, this));
        return CreateComplexType(type);
    }

    private object CreateComplexType(Type type)
    {
        if (_activeTypes.Contains(type)) return null;
        _activeTypes.Add(type);

        try
        {
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .OrderByDescending(c => c.GetParameters().Length);
            object instance = null;
            ParameterInfo[] usedParameters = Array.Empty<ParameterInfo>();

            foreach (var ctor in ctors)
            {
                try
                {
                    usedParameters = ctor.GetParameters();
                    var args = usedParameters.Select(p => Create(p.ParameterType, p.Name, type)).ToArray();
                    instance = ctor.Invoke(args);
                    break; 
                }
                catch { continue; } 
            }

            if (instance == null) return null;
            
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanWrite && !usedParameters.Any(up => up.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
            
            foreach (var prop in props)
            {
                prop.SetValue(instance, Create(prop.PropertyType, prop.Name, type));
            }

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                             .Where(f => !usedParameters.Any(up => up.Name.Equals(f.Name, StringComparison.OrdinalIgnoreCase)));
            
            foreach (var field in fields)
            {
                field.SetValue(instance, Create(field.FieldType, field.Name, type));
            }

            return instance;
        }
        finally
        {
            _activeTypes.Remove(type);
        }
    }
}

