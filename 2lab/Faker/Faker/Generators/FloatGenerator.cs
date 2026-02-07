namespace Faker.Generators;

public class FloatGenerator : IValueGenerator
{
    public bool CanGenerate(Type type) => type == typeof(float);

    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        return (float)(context.Random.NextDouble() * float.MaxValue);
    }
}