namespace Pokedle.Api.Domain;

public class Pokemon
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Generation { get; set; }
    public int EvolutionStage { get; set; }
    public int HabitatId { get; set; }
    public Habitat? Habitat { get; set; }
    public int ColorId { get; set; }
    public PokemonColor? Color { get; set; }
    public ICollection<PokemonElementType> PokemonElementTypes { get; set; } = [];
}
