using AutoFixture;
using AutoFixture.AutoNSubstitute;
using HotChocolate;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pokedle.Api.Domain;
using Pokedle.Api.GraphQL;
using Pokedle.Api.Infrastructure;
using Pokedle.Api.Services;
using Shouldly;
using Xunit;

namespace Pokedle.Api.Tests.GraphQL;

public class MutationGuessTests
{
    private readonly IFixture _fixture;
    private readonly ITopicEventSender _sender;
    private readonly ILogger<Mutation> _logger;

    public MutationGuessTests()
    {
        _fixture = TestHelpers.CreateFixture();
        _sender = Substitute.For<ITopicEventSender>();
        _logger = Substitute.For<ILogger<Mutation>>();
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    /// <summary>
    /// Builds a (daily, guess) pair using AutoFixture for all string/numeric data.
    /// Use sameId=true to simulate a correct guess.
    /// </summary>
    private (Pokemon daily, Pokemon guess) BuildPair(
        bool sameGeneration = true,
        bool sameHabitat = true,
        bool sameColor = true,
        bool sameId = false
    )
    {
        var elementType = new ElementType { Id = 1, Name = _fixture.Create<string>() };

        var daily = _fixture
            .Build<Pokemon>()
            .With(p => p.Id, 4)
            .With(p => p.Name, _fixture.Create<string>().ToLower())
            .With(p => p.Generation, 1)
            .With(p => p.EvolutionStage, 1)
            .With(p => p.HabitatId, 2)
            .With(p => p.Habitat, new Habitat { Id = 2, Name = _fixture.Create<string>() })
            .With(p => p.ColorId, 3)
            .With(p => p.Color, new PokemonColor { Id = 3, Name = _fixture.Create<string>() })
            .With(
                p => p.PokemonElementTypes,
                new List<PokemonElementType>
                {
                    new()
                    {
                        PokemonId = 4,
                        ElementTypeId = 1,
                        ElementType = elementType,
                    },
                }
            )
            .Create();

        var guessId = sameId ? 4 : 25;
        var guess = _fixture
            .Build<Pokemon>()
            .With(p => p.Id, guessId)
            .With(p => p.Name, sameId ? daily.Name : _fixture.Create<string>().ToLower())
            .With(p => p.Generation, sameGeneration ? daily.Generation : daily.Generation + 1)
            .With(p => p.EvolutionStage, 1)
            .With(p => p.HabitatId, sameHabitat ? 2 : 99)
            .With(
                p => p.Habitat,
                sameHabitat
                    ? new Habitat { Id = 2, Name = daily.Habitat!.Name }
                    : new Habitat { Id = 99, Name = _fixture.Create<string>() }
            )
            .With(p => p.ColorId, sameColor ? 3 : 10)
            .With(
                p => p.Color,
                sameColor
                    ? new PokemonColor { Id = 3, Name = daily.Color!.Name }
                    : new PokemonColor { Id = 10, Name = _fixture.Create<string>() }
            )
            .With(
                p => p.PokemonElementTypes,
                new List<PokemonElementType>
                {
                    new()
                    {
                        PokemonId = guessId,
                        ElementTypeId = 1,
                        ElementType = elementType,
                    },
                }
            )
            .Create();

        return (daily, guess);
    }

    /// <summary>
    /// Builds the InMemory context seeded with the guessed Pokémon row.
    /// </summary>
    private static PokedleContext BuildGuessContext(string dbName, Pokemon daily, Pokemon guess)
    {
        var context = TestHelpers.CreateInMemoryContext(dbName);

        context.ElementTypes.Add(new ElementType { Id = 1, Name = "fire" });

        context.Habitats.Add(new Habitat { Id = guess.HabitatId, Name = guess.Habitat!.Name });
        if (daily.HabitatId != guess.HabitatId)
            context.Habitats.Add(new Habitat { Id = daily.HabitatId, Name = daily.Habitat!.Name });

        context.Colors.Add(new PokemonColor { Id = guess.ColorId, Name = guess.Color!.Name });
        if (daily.ColorId != guess.ColorId)
            context.Colors.Add(new PokemonColor { Id = daily.ColorId, Name = daily.Color!.Name });

        context.Pokemons.Add(
            new Pokemon
            {
                Id = guess.Id,
                Name = guess.Name,
                Generation = guess.Generation,
                EvolutionStage = guess.EvolutionStage,
                HabitatId = guess.HabitatId,
                ColorId = guess.ColorId,
                PokemonElementTypes =
                [
                    new PokemonElementType { PokemonId = guess.Id, ElementTypeId = 1 },
                ],
            }
        );

        context.SaveChanges();
        return context;
    }

    /// <summary>
    /// Builds the InMemory context for DailyPokemonService.
    /// Seeded with exactly one Pokémon at Id=1 so rng.Next(1,1)
    /// always resolves deterministically without requiring a virtual method.
    /// </summary>
    private static PokedleContext BuildDailyContext(string dbName, Pokemon daily)
    {
        var context = TestHelpers.CreateInMemoryContext(dbName);

        context.ElementTypes.Add(new ElementType { Id = 1, Name = "fire" });
        context.Habitats.Add(new Habitat { Id = daily.HabitatId, Name = daily.Habitat!.Name });
        context.Colors.Add(new PokemonColor { Id = daily.ColorId, Name = daily.Color!.Name });
        context.Pokemons.Add(
            new Pokemon
            {
                Id = 1,
                Name = daily.Name,
                Generation = daily.Generation,
                EvolutionStage = daily.EvolutionStage,
                HabitatId = daily.HabitatId,
                ColorId = daily.ColorId,
                PokemonElementTypes = [new PokemonElementType { PokemonId = 1, ElementTypeId = 1 }],
            }
        );

        context.SaveChanges();
        return context;
    }

    private async Task<GuessResult> RunGuess(Pokemon daily, Pokemon guess)
    {
        var guessContext = BuildGuessContext($"guess-{Guid.NewGuid()}", daily, guess);
        var dailyContext = BuildDailyContext($"daily-{Guid.NewGuid()}", daily);
        var dailyService = new DailyPokemonService(dailyContext);

        var sut = new Mutation();
        return await sut.Guess(guess.Name, dailyService, guessContext, _logger, _sender);
    }

    // ---------------------------------------------------------------
    // Tests
    // ---------------------------------------------------------------

    [Fact]
    public async Task Guess_CorrectPokemon_IsCorrect_True()
    {
        var (daily, guess) = BuildPair(sameId: true);
        // Daily context seeds at Id=1; align both to Id=1 for IsCorrect match
        daily.Id = 1;
        guess.Id = 1;

        var result = await RunGuess(daily, guess);

        result.IsCorrect.ShouldBeTrue();
    }

    [Fact]
    public async Task Guess_WrongPokemon_IsCorrect_False()
    {
        var (daily, guess) = BuildPair(sameId: false);
        var result = await RunGuess(daily, guess);

        result.IsCorrect.ShouldBeFalse();
    }

    [Fact]
    public async Task Guess_SameGeneration_Returns_Correct_Hint()
    {
        var (daily, guess) = BuildPair(sameGeneration: true);
        var result = await RunGuess(daily, guess);

        result.Generation.ShouldBe(Hint.Correct);
    }

    [Fact]
    public async Task Guess_LowerGeneration_Returns_Higher_Hint()
    {
        // guess.Generation < daily.Generation → Hint.Higher (arrow points up)
        var (daily, guess) = BuildPair(sameGeneration: false);
        daily.Generation = 2;
        guess.Generation = 1;

        var result = await RunGuess(daily, guess);

        result.Generation.ShouldBe(Hint.Higher);
    }

    [Fact]
    public async Task Guess_HigherGeneration_Returns_Lower_Hint()
    {
        // guess.Generation > daily.Generation → Hint.Lower (arrow points down)
        var (daily, guess) = BuildPair(sameGeneration: false);
        daily.Generation = 1;
        guess.Generation = 3;

        var result = await RunGuess(daily, guess);

        result.Generation.ShouldBe(Hint.Lower);
    }

    [Fact]
    public async Task Guess_SameHabitat_Returns_Correct_Hint()
    {
        var (daily, guess) = BuildPair(sameHabitat: true);
        var result = await RunGuess(daily, guess);

        result.Habitat.ShouldBe(Hint.Correct);
    }

    [Fact]
    public async Task Guess_DifferentHabitat_Returns_Wrong_Hint()
    {
        var (daily, guess) = BuildPair(sameHabitat: false);
        var result = await RunGuess(daily, guess);

        result.Habitat.ShouldBe(Hint.Wrong);
    }

    [Fact]
    public async Task Guess_SameColor_Returns_Correct_Hint()
    {
        var (daily, guess) = BuildPair(sameColor: true);
        var result = await RunGuess(daily, guess);

        result.Color.Hint.ShouldBe(Hint.Correct);
    }

    [Fact]
    public async Task Guess_DifferentColor_Returns_Wrong_Hint()
    {
        var (daily, guess) = BuildPair(sameColor: false);
        var result = await RunGuess(daily, guess);

        result.Color.Hint.ShouldBe(Hint.Wrong);
    }

    [Fact]
    public async Task Guess_GuessPokemonName_Matches_Input()
    {
        var (daily, guess) = BuildPair();
        var result = await RunGuess(daily, guess);

        result.GuessPokemon.ShouldBe(guess.Name);
    }

    [Fact]
    public async Task Guess_Result_Has_NonNull_Types()
    {
        var (daily, guess) = BuildPair();
        var result = await RunGuess(daily, guess);

        result.Types.ShouldNotBeNull();
        result.Types.Slot1.ShouldNotBeNull();
        result.Types.Slot2.ShouldNotBeNull();
    }

    [Fact]
    public async Task Guess_Sends_Event_Via_Sender()
    {
        var (daily, guess) = BuildPair();
        await RunGuess(daily, guess);

        await _sender
            .Received(1)
            .SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Guess_Unknown_Pokemon_Throws_GraphQLException()
    {
        var (daily, _) = BuildPair();
        var dailyContext = BuildDailyContext($"daily-{Guid.NewGuid()}", daily);
        var emptyGuessContext = TestHelpers.CreateInMemoryContext($"empty-{Guid.NewGuid()}");
        var dailyService = new DailyPokemonService(dailyContext);
        var sut = new Mutation();

        await Should.ThrowAsync<GraphQLException>(() =>
            sut.Guess("unknownmon", dailyService, emptyGuessContext, _logger, _sender)
        );
    }
}
