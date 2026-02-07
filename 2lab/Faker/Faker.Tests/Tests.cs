using Faker;
using Xunit;

namespace Faker.Tests;

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public class A
{
    public B B { get; set; }
}

public class B
{
    public C C { get; set; }
}

public class C
{
    public A A { get; set; }
}

public class Person
{
    public string Name { get; }

    public Person(string name)
    {
        Name = name;
    }
}

public class FakerTests
{
    [Fact]
    public void Create_SimpleObject_ReturnsFilledObject()
    {
        var faker = new Faker();
        var user = faker.Create<User>();

        Assert.NotNull(user.Name);
        Assert.NotEqual(0, user.Age);
    }

    [Fact]
    public void Create_CircularDependency_ReturnsNullOnCycle()
    {
        var faker = new Faker();
        var a = faker.Create<A>();

        Assert.NotNull(a.B);
        Assert.NotNull(a.B.C);
        Assert.Null(a.B.C.A);
    }

    [Fact]
    public void Create_WithConfig_UsesCustomGenerator()
    {
        var config = new FakerConf();
        config.Add<User, string, ConstantNameGenerator>(u => u.Name);
        var faker = new Faker(config);

        var user = faker.Create<User>();

        Assert.Equal("ConstantValue", user.Name);
    }
    
    [Fact]
    public void Create_List_ReturnsFilledList()
    {
        var faker = new Faker();
        var users = faker.Create<List<User>>();

        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.All(users, u =>
        {
            Assert.NotNull(u.Name);
            Assert.NotEqual(0, u.Age);
        });
    }

    [Fact]
    public void Create_Int_ReturnsRandomValue()
    {
        var faker = new Faker();
        var value = faker.Create<int>();
        Assert.InRange(value, int.MinValue, int.MaxValue);
    }

    [Fact]
    public void Create_Double_ReturnsRandomValue()
    {
        var faker = new Faker();
        var value = faker.Create<double>();
        Assert.InRange(value, 0.0, double.MaxValue);
    }
    
    [Fact]
    public void Create_DateTime_ReturnsRandomValue()
    {
        var faker = new Faker();
        var value = faker.Create<DateTime>();
        Assert.NotEqual(default, value);
    }

    [Fact]
    public void Create_WithConfig_ImmutablePerson_UsesCustomGeneratorForConstructor()
    {
        var config = new FakerConf();
        config.Add<Person, string, ConstantNameGenerator>(p => p.Name);
        var faker = new Faker(config);

        var person = faker.Create<Person>();

        Assert.NotNull(person);
        Assert.Equal("ConstantValue", person.Name);
    }
}

public class ConstantNameGenerator : IValueGenerator {
    public bool CanGenerate(Type t) => t == typeof(string);
    public object Generate(Type t, GeneratorContext c) => "ConstantValue";
}