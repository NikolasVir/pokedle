using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Domain;
using Pokedle.Api.Infrastructure;

namespace Pokedle.Api.Services;

/// <summary>Determines the daily Pokémon based on the current date.</summary>
public class DailyPokemonService(PokedleContext context)
{
    /// <summary>Returns the Pokémon of the day seeded by UTC date.</summary>
    public async Task<Pokemon> GetDailyPokemonAsync()
    {
        var pokemonCount = await context.Pokemons.CountAsync(p =>
            p.Id < PokedleContext.MaxBasePokemonId
        );
        // Seed with today's date so the same Pokémon is returned for the entire day
        var seed = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
        var rng = new Random(seed);
        var index = rng.Next(1, pokemonCount);

        var daily =
            await context
                .Pokemons.Include(p => p.Habitat)
                .Include(p => p.Color)
                .Include(p => p.PokemonElementTypes)
                    .ThenInclude(pet => pet.ElementType)
                .FirstOrDefaultAsync(p => p.Id == index)
            ?? throw new Exception("Daily Pokemon not Found");

        return daily;
    }
}
