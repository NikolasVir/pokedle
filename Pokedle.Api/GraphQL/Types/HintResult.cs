namespace Pokedle.Api.GraphQL;

/// <summary>Hint outcome for a single attribute.</summary>
public enum Hint
{
    /// <summary>The guess matches exactly.</summary>
    Correct,
    /// <summary>The guess is partially correct.</summary>
    Partial,
    /// <summary>The guess is wrong.</summary>
    Wrong,
    /// <summary>The answer is higher than the guess.</summary>
    Higher,
    /// <summary>The answer is lower than the guess.</summary>
    Lower
}

/// <summary>Result of comparing a guess against the daily Pokémon.</summary>
public class GuessResult
{
    /// <summary>Name of the guessed Pokémon.</summary>
    public string GuessPokemon { get; set; } = default!;
    /// <summary>Generation hint.</summary>
    public Hint Generation { get; set; }
    /// <summary>Habitat hint.</summary>
    public Hint Habitat { get; set; }
    /// <summary>Color comparison.</summary>
    public ColorResult Color { get; set; } = null!;
    /// <summary>Elemental type comparison.</summary>
    public TypeHintResult Types { get; set; } = default!;
    /// <summary>Whether the guess is the correct Pokémon.</summary>
    public bool IsCorrect { get; set; }
}
