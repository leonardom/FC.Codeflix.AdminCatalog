using Bogus;

namespace FC.Codeflix.AdminCatalog.UnitTests.Common;

public abstract class BaseFixture
{
    public Faker Faker { get; init; } = new();
}