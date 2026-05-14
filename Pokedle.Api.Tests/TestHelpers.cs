using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Infrastructure;

namespace Pokedle.Api.Tests;

public static class TestHelpers
{
    /// <summary>Creates an AutoFixture instance pre-configured with NSubstitute.</summary>
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture().Customize(
            new AutoNSubstituteCustomization { ConfigureMembers = true }
        );
        fixture
            .Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }

    /// <summary>Creates a fresh InMemory PokedleContext for each test.</summary>
    public static PokedleContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PokedleContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new PokedleContext(options);
    }
}
