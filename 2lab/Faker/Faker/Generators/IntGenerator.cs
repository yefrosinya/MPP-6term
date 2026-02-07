namespace Faker.Generators;

public class IntGenerator : IValueGenerator
{
    public bool CanGenerate(Type type) => type == typeof(int);

    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        return (int)context.Random.NextInt64(int.MinValue, (long)int.MaxValue + 1);
    }
}