namespace Faker;
using System.Linq.Expressions;

public class FakerConf
{
    private readonly Dictionary<Type, Dictionary<string, Type>> _customGenerators = new();

    public void Add<TClass, TProp, TGen>(Expression<Func<TClass, TProp>> selector) 
        where TGen : IValueGenerator
    {
        if (selector.Body is not MemberExpression member)
            throw new ArgumentException("Expression must be a property or field access");

        var type = typeof(TClass);
        var memberName = member.Member.Name;

        if (!_customGenerators.ContainsKey(type))
            _customGenerators[type] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        _customGenerators[type][memberName] = typeof(TGen);
    }

    public Type GetGeneratorType(Type classType, string memberName)
    {
        if (_customGenerators.TryGetValue(classType, out var members) && 
            members.TryGetValue(memberName, out var genType))
            return genType;
        return null;
    }
}