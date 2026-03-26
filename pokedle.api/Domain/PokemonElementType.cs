namespace pokedle.api.Domain;

public class PokemonElementType
{
    public int Id { get; set; }
    public int PokemonId { get; set; }
    public Pokemon? Pokemon { get; set; }
    public int ElementTypeId { get; set; }
    public ElementType? ElementType { get; set; }
    public int Slot { get; set; }
}
