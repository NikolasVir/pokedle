namespace Pokedle.Api.Domain;

/// <summary>A Pokémon habitat (e.g. forest, cave).</summary>
public class Habitat
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }
    /// <summary>Habitat name.</summary>
    public required string Name { get; set; }
}
