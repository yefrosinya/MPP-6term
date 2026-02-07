namespace Faker.Generators;

public class StringGenerator : IValueGenerator
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public bool CanGenerate(Type type) => type == typeof(string);

    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        int length = context.Random.Next(5, 15);
        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = Alphabet[context.Random.Next(Alphabet.Length)];
        }
        return new string(chars);
    }
}