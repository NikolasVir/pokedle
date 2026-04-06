namespace Pokedle.Api.GraphQL;

public enum Hint
{
    Correct,
    Partial,
    Wrong,
    Higher,  // the answer is higher than the guess
    Lower    // the answer is lower than the guess
}

public class GuessResult
{
    public string GuessPokemon { get; set; } = default!;
    public Hint Generation { get; set; }
    public Hint Habitat { get; set; }
    public ColorResult Color { get; set; } = null!;
    public TypeHintResult Types { get; set; } = default!;
    public bool IsCorrect { get; set; }
}