using Bogus;

namespace FC.Codeflix.AdminCatalog.UnitTests.Common;

public abstract class BaseFixture
{
    public Faker Faker { get; } = new();

    private string GenerateString(int minLength, int maxLength)
    {
        maxLength = minLength > maxLength ? minLength : maxLength;
        var value = Faker.Lorem.Sentence();
        while (value.Length < minLength)
        {
            value += Faker.Lorem.Sentence();
        }
        return value[..(value.Length > maxLength ? maxLength : value.Length)];
    }
    
    public string GenerateName(int minLength = 3, int maxLength = 255)
        => GenerateString(minLength, maxLength);

    public string GenerateDescription(int minLength = 100, int maxLength = 10_000)
        => GenerateString(minLength, maxLength);
}