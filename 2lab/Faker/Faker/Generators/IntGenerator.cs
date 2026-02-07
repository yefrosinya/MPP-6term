namespace Faker.Generators;

public class IntGenerator : IValueGenerator
{
    public bool CanGenerate(Type type) => type == typeof(int);

    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        return context.Random.Next(int.MinValue, int.MaxValue);
    }
}