namespace Faker;
using System.Linq.Expressions;
using System.Collections.Concurrent;

public class FakerConf
{
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Type>> _customGenerators = new();

    public void Add<TClass, TProp, TGen>(Expression<Func<TClass, TProp>> selector) 
        where TGen : IValueGenerator
    {
        if (selector.Body is not MemberExpression member)
            throw new ArgumentException("Expression must be a property or field access");

        var type = typeof(TClass);
        var memberName = member.Member.Name;

        var members = _customGenerators.GetOrAdd(type, _ => new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase));
        members[memberName] = typeof(TGen);
    }
    
    public Type GetGeneratorType(Type classType, string memberName)
    {
        if (_customGenerators.TryGetValue(classType, out var members) && 
            members.TryGetValue(memberName, out var genType))
            return genType;
        return null;
    }
}