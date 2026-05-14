using AutoFixture;
using Pokedle.Api.Domain;
using Pokedle.Api.Infrastructure;
using Pokedle.Api.Services;
using Shouldly;
using Xunit;

namespace Pokedle.Api.Tests.Services;

public class DailyPokemonServiceTests
{
    private readonly IFixture _fixture = TestHelpers.CreateFixture();

    private PokedleContext BuildContext(string dbName)
    {
        var context = TestHelpers.CreateInMemoryContext(dbName);

        context.Habitats.Add(new Habitat { Id = 1, Name = _fixture.Create<string>() });
        context.Colors.Add(new PokemonColor { Id = 1, Name = _fixture.Create<string>() });
        context.ElementTypes.Add(new ElementType { Id = 1, Name = _fixture.Create<string>() });

        for (int i = 1; i <= 10; i++)
        {
            context.Pokemons.Add(
                new Pokemon
                {
                    Id = i,
                    Name = _fixture.Create<string>().ToLower(),
                    Generation = 1,
                    EvolutionStage = 1,
                    HabitatId = 1,
                    ColorId = 1,
                    PokemonElementTypes =
                    [
                        new PokemonElementType { PokemonId = i, ElementTypeId = 1 },
                    ],
                }
            );
        }

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetDailyPokemonAsync_Returns_NonNull_Pokemon()
    {
        // Arrange
        var context = BuildContext(nameof(GetDailyPokemonAsync_Returns_NonNull_Pokemon));
        var sut = new DailyPokemonService(context);

        // Act
        var result = await sut.GetDailyPokemonAsync();

        // Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetDailyPokemonAsync_Returns_Pokemon_With_Name()
    {
        var context = BuildContext(nameof(GetDailyPokemonAsync_Returns_Pokemon_With_Name));
        var sut = new DailyPokemonService(context);

        var result = await sut.GetDailyPokemonAsync();

        result.Name.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetDailyPokemonAsync_Is_Deterministic_For_Same_Day()
    {
        // Same DB + same date seed → always returns the same Pokémon
        var context = BuildContext(nameof(GetDailyPokemonAsync_Is_Deterministic_For_Same_Day));
        var sut = new DailyPokemonService(context);

        var first = await sut.GetDailyPokemonAsync();
        var second = await sut.GetDailyPokemonAsync();

        first.Id.ShouldBe(second.Id);
    }

    [Fact]
    public async Task GetDailyPokemonAsync_Returns_Pokemon_With_Id_In_Range()
    {
        var context = BuildContext(nameof(GetDailyPokemonAsync_Returns_Pokemon_With_Id_In_Range));
        var sut = new DailyPokemonService(context);

        var result = await sut.GetDailyPokemonAsync();

        result.Id.ShouldBeInRange(1, 10);
    }

    [Fact]
    public async Task GetDailyPokemonAsync_Returns_Pokemon_With_Habitat_Populated()
    {
        var context = BuildContext(
            nameof(GetDailyPokemonAsync_Returns_Pokemon_With_Habitat_Populated)
        );
        var sut = new DailyPokemonService(context);

        var result = await sut.GetDailyPokemonAsync();

        result.Habitat.ShouldNotBeNull();
        result.Habitat!.Name.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetDailyPokemonAsync_Returns_Pokemon_With_Color_Populated()
    {
        var context = BuildContext(
            nameof(GetDailyPokemonAsync_Returns_Pokemon_With_Color_Populated)
        );
        var sut = new DailyPokemonService(context);

        var result = await sut.GetDailyPokemonAsync();

        result.Color.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetDailyPokemonAsync_Throws_When_No_Pokemon_In_Db()
    {
        // Empty DB → service must throw
        var context = TestHelpers.CreateInMemoryContext(
            nameof(GetDailyPokemonAsync_Throws_When_No_Pokemon_In_Db)
        );
        var sut = new DailyPokemonService(context);

        await Should.ThrowAsync<Exception>(() => sut.GetDailyPokemonAsync());
    }
}
