namespace Pokedle.Api.Domain;

/// <summary>A Pokémon species.</summary>
public class Pokemon
{
    /// <summary>National Pokédex number.</summary>
    public int Id { get; set; }
    /// <summary>Pokémon name (lowercase).</summary>
    public required string Name { get; set; }
    /// <summary>Game generation (1-8).</summary>
    public int Generation { get; set; }
    /// <summary>Depth in evolution chain.</summary>
    public int EvolutionStage { get; set; }
    /// <summary>Foreign key to Habitat.</summary>
    public int HabitatId { get; set; }
    /// <summary>Navigation property to Habitat.</summary>
    public Habitat? Habitat { get; set; }
    /// <summary>Foreign key to PokemonColor.</summary>
    public int ColorId { get; set; }
    /// <summary>Navigation property to PokemonColor.</summary>
    public PokemonColor? Color { get; set; }
    /// <summary>Join table for elemental types.</summary>
    public ICollection<PokemonElementType> PokemonElementTypes { get; set; } = [];
}
