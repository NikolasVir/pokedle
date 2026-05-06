using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedle.Api.Domain;

/// <summary>A logged guess submission.</summary>
public class GuessHistory
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }
    /// <summary>Anonymous client identifier.</summary>
    public required string ClientId { get; set; }
    /// <summary>When the guess was made.</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    /// <summary>ID of the daily Pokémon.</summary>
    public int DailyPokemonId { get; set; }
    /// <summary>ID of the guessed Pokémon.</summary>
    public int GuessedPokemonId { get; set; }

    /// <summary>JSON-serialized guess result.</summary>
    [Column(TypeName = "jsonb")]
    public required string Results { get; set; }
}
