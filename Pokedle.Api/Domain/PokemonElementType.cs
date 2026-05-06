namespace Pokedle.Api.Domain;

/// <summary>Join entity linking a Pokémon to its elemental type(s).</summary>
public class PokemonElementType
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }
    /// <summary>Foreign key to Pokemon.</summary>
    public int PokemonId { get; set; }
    /// <summary>Navigation property to Pokemon.</summary>
    public Pokemon? Pokemon { get; set; }
    /// <summary>Foreign key to ElementType.</summary>
    public int ElementTypeId { get; set; }
    /// <summary>Navigation property to ElementType.</summary>
    public ElementType? ElementType { get; set; }
    /// <summary>Slot order (1 or 2).</summary>
    public int Slot { get; set; }
}
