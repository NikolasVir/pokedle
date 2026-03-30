using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedle.Api.Domain;

public class GuessHistory
{
    public int Id { get; set; }
    public required string ClientId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int DailyPokemonId { get; set; }
    public int GuessedPokemonId { get; set; }

    [Column(TypeName = "jsonb")]
    public required string Results { get; set; }
}
