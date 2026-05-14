using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Domain;
using Pokedle.Api.Infrastructure;

namespace Pokedle.Api.Tests;

public static class TestHelpers
{
    /// <summary>Creates an AutoFixture instance pre-configured with NSubstitute.</summary>
    public static IFixture CreateFixture() =>
        new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });

    /// <summary>Creates a fresh InMemory PokedleContext for each test.</summary>
    public static PokedleContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PokedleContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new PokedleContext(options);
    }

    /// <summary>Builds a minimal valid Pokemon with navigations populated.</summary>
    public static Pokemon BuildPokemon(int id, string name, int generation = 1)
    {
        var habitat = new Habitat { Id = 1, Name = "grassland" };
        var color = new PokemonColor { Id = 1, Name = "yellow" };
        var elementType = new ElementType { Id = 1, Name = "electric" };

        var pokemon = new Pokemon
        {
            Id = id,
            Name = name,
            Generation = generation,
            EvolutionStage = 1,
            HabitatId = habitat.Id,
            Habitat = habitat,
            ColorId = color.Id,
            Color = color,
            PokemonElementTypes =
            [
                new PokemonElementType
                {
                    PokemonId = id,
                    ElementTypeId = elementType.Id,
                    ElementType = elementType,
                },
            ],
        };

        return pokemon;
    }
}
