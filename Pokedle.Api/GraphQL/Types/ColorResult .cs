using Pokedle.Api.GraphQL;

public class ColorResult
{
    public string ColorName { get; set; } = null!;
    public Hint Hint { get; set; }
}