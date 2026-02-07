namespace Faker;
using System.Collections;

public class ListGenerator : IValueGenerator
{
    public bool CanGenerate(Type type) => 
        type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || 
                               type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                               type.GetGenericTypeDefinition() == typeof(IList<>));

    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        var itemType = typeToGenerate.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(itemType);
        var list = (IList)Activator.CreateInstance(listType);

        int count = context.Random.Next(2, 5); 
        for (int i = 0; i < count; i++)
        {
            list.Add(context.Faker.Create(itemType)); 
        }
        return list;
    }
}