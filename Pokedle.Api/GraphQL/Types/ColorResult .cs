using Pokedle.Api.GraphQL;

/// <summary>Color comparison result.</summary>
public class ColorResult
{
    /// <summary>The guessed Pokémon's color.</summary>
    public string ColorName { get; set; } = null!;
    /// <summary>Correct or Wrong.</summary>
    public Hint Hint { get; set; }
}
