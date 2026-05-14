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
        _fixture = new Fixture().Customize(
            new AutoNSubstituteCustomization { ConfigureMembers = true }
        );
        _sender = Substitute.For<ITopicEventSender>();
        _logger = Substitute.For<ILogger<Mutation>>();
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    /// <summary>
    /// Creates a fresh InMemory context pre-seeded with the two given Pokémon
    /// (daily is only added when it differs from guess).
    /// </summary>
    private static PokedleContext BuildContext(string dbName, Pokemon daily, Pokemon guess)
    {
        var context = TestHelpers.CreateInMemoryContext(dbName);

        // Always add the element type
        context.ElementTypes.Add(new ElementType { Id = 1, Name = "fire" });

        // Habitats
        context.Habitats.Add(new Habitat { Id = guess.HabitatId, Name = guess.Habitat!.Name });
        if (daily.HabitatId != guess.HabitatId)
            context.Habitats.Add(new Habitat { Id = daily.HabitatId, Name = daily.Habitat!.Name });

        // Colors
        context.Colors.Add(new PokemonColor { Id = guess.ColorId, Name = guess.Color!.Name });
        if (daily.ColorId != guess.ColorId)
            context.Colors.Add(new PokemonColor { Id = daily.ColorId, Name = daily.Color!.Name });

        // The guessed Pokémon row (without nav props — EF resolves via FKs)
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
    /// Builds a (daily, guess) pair. Use sameId=true to simulate a correct guess.
    /// </summary>
    private static (Pokemon daily, Pokemon guess) BuildPair(
        bool sameGeneration = true,
        bool sameHabitat = true,
        bool sameColor = true,
        bool sameId = false
    )
    {
        var elementType = new ElementType { Id = 1, Name = "fire" };

        var daily = new Pokemon
        {
            Id = 4,
            Name = "charmander",
            Generation = 1,
            EvolutionStage = 1,
            HabitatId = 2,
            Habitat = new Habitat { Id = 2, Name = "mountain" },
            ColorId = 3,
            Color = new PokemonColor { Id = 3, Name = "red" },
            PokemonElementTypes =
            [
                new PokemonElementType
                {
                    PokemonId = 4,
                    ElementTypeId = 1,
                    ElementType = elementType,
                },
            ],
        };

        var guess = new Pokemon
        {
            Id = sameId ? 4 : 25,
            Name = sameId ? "charmander" : "pikachu",
            Generation = sameGeneration ? 1 : 2,
            EvolutionStage = 1,
            HabitatId = sameHabitat ? 2 : 99,
            Habitat = sameHabitat
                ? new Habitat { Id = 2, Name = "mountain" }
                : new Habitat { Id = 99, Name = "forest" },
            ColorId = sameColor ? 3 : 10,
            Color = sameColor
                ? new PokemonColor { Id = 3, Name = "red" }
                : new PokemonColor { Id = 10, Name = "yellow" },
            PokemonElementTypes =
            [
                new PokemonElementType
                {
                    PokemonId = sameId ? 4 : 25,
                    ElementTypeId = 1,
                    ElementType = elementType,
                },
            ],
        };

        return (daily, guess);
    }

    /// <summary>
    /// Runs Mutation.Guess end-to-end using a real InMemory DailyPokemonService
    /// that is pre-loaded to return <paramref name="daily"/>.
    /// </summary>
    private async Task<GuessResult> RunGuess(Pokemon daily, Pokemon guess)
    {
        var dbName = $"guess-{Guid.NewGuid()}";
        var context = BuildContext(dbName, daily, guess);

        // Use InMemory context for DailyPokemonService too, but seed it
        // so the deterministic random picks the correct daily pokemon.
        // Easier: use a second context instance pointing at the same DB
        // and just create a substituted wrapper around the real service.
        // We use a separate InMemory DB seeded only with the daily pokemon
        // so GetDailyPokemonAsync always returns it.
        var dailyDbName = $"daily-{Guid.NewGuid()}";
        var dailyContext = TestHelpers.CreateInMemoryContext(dailyDbName);

        dailyContext.ElementTypes.Add(new ElementType { Id = 1, Name = "fire" });
        dailyContext.Habitats.Add(new Habitat { Id = daily.HabitatId, Name = daily.Habitat!.Name });
        dailyContext.Colors.Add(new PokemonColor { Id = daily.ColorId, Name = daily.Color!.Name });

        // Seed exactly 1 pokemon so index always == daily.Id
        // The service picks: index = rng.Next(1, count) where count==1, so index is always 1
        // We therefore need Id=1 for this seeding trick to work.
        // Instead, we subclass/wrap using a partial NSubstitute spy on the real service:
        // Simplest approach — NSubstitute the abstract method via a real instance partial mock.
        // Because DailyPokemonService is NOT virtual/interface, we mock at the call site:
        // create a substitute of an *interface* we extract — but we don't have one.
        // Best solution: seed dailyContext with ONLY the daily pokemon at Id=1
        // and adjust count so rng.Next(1,1)=1 always resolves Id=1.
        dailyContext.Pokemons.Add(
            new Pokemon
            {
                Id = 1, // ← seed at Id=1 so rng always picks it
                Name = daily.Name,
                Generation = daily.Generation,
                EvolutionStage = daily.EvolutionStage,
                HabitatId = daily.HabitatId,
                ColorId = daily.ColorId,
                PokemonElementTypes = [new PokemonElementType { PokemonId = 1, ElementTypeId = 1 }],
            }
        );
        dailyContext.SaveChanges();

        var dailyService = new DailyPokemonService(dailyContext);

        var sut = new Mutation();
        return await sut.Guess(guess.Name, dailyService, context, _logger, _sender);
    }

    // ---------------------------------------------------------------
    // Tests
    // ---------------------------------------------------------------

    [Fact]
    public async Task Guess_CorrectPokemon_IsCorrect_True()
    {
        var (daily, guess) = BuildPair(sameId: true);
        // For IsCorrect we need the guessed pokemon to have the same Id as daily.
        // RunGuess seeds the daily at Id=1 and the guess also at Id=4 in main context.
        // So to make IsCorrect=true we need guess.Id == daily.Id inside the service.
        // We override: make daily.Id=1 so it matches what the service returns.
        daily.Id = 1;
        guess.Id = 1;
        guess.Name = daily.Name;

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
        // guess.Generation(1) < daily.Generation(2) → Hint.Higher
        var (daily, guess) = BuildPair(sameGeneration: false);
        daily.Generation = 2;
        guess.Generation = 1;

        var result = await RunGuess(daily, guess);

        result.Generation.ShouldBe(Hint.Higher);
    }

    [Fact]
    public async Task Guess_HigherGeneration_Returns_Lower_Hint()
    {
        // guess.Generation(3) > daily.Generation(1) → Hint.Lower
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

        // NSubstitute: verify SendAsync was called at least once with any args
        await _sender
            .Received(1)
            .SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Guess_Unknown_Pokemon_Throws_GraphQLException()
    {
        var (daily, _) = BuildPair();

        // Build a context with the daily seeded, but do NOT add "unknownmon"
        var dailyDbName = $"daily-{Guid.NewGuid()}";
        var dailyCtx = TestHelpers.CreateInMemoryContext(dailyDbName);
        dailyCtx.ElementTypes.Add(new ElementType { Id = 1, Name = "fire" });
        dailyCtx.Habitats.Add(new Habitat { Id = daily.HabitatId, Name = daily.Habitat!.Name });
        dailyCtx.Colors.Add(new PokemonColor { Id = daily.ColorId, Name = daily.Color!.Name });
        dailyCtx.Pokemons.Add(
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
        dailyCtx.SaveChanges();

        var emptyGuessCtx = TestHelpers.CreateInMemoryContext($"empty-{Guid.NewGuid()}");
        var dailyService = new DailyPokemonService(dailyCtx);
        var sut = new Mutation();

        await Should.ThrowAsync<GraphQLException>(() =>
            sut.Guess("unknownmon", dailyService, emptyGuessCtx, _logger, _sender)
        );
    }
}
