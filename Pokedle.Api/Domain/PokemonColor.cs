namespace Pokedle.Api.Domain;

/// <summary>A Pokémon color category.</summary>
public class PokemonColor
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }
    /// <summary>Color name.</summary>
    public required string Name { get; set; }
}
