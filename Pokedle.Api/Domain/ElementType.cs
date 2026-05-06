namespace Pokedle.Api.Domain;

/// <summary>A Pokémon elemental type (e.g. fire, water).</summary>
public class ElementType
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }
    /// <summary>Type name.</summary>
    public required string Name { get; set; }
}
