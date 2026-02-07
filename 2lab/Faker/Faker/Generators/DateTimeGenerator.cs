namespace Faker.Generators;

public class DateTimeGenerator : IValueGenerator
{
    public bool CanGenerate(Type type) => type == typeof(DateTime);

    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        var start = new DateTime(1970, 1, 1);
        var range = (DateTime.MaxValue - start).Ticks;
        var randomTicks = (long)(context.Random.NextDouble() * range);
        return start.AddTicks(randomTicks);
    }
}