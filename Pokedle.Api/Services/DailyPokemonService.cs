using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Domain;
using Pokedle.Api.Infrastructure;

namespace Pokedle.Api.Services;

public class DailyPokemonService(PokedleContext context)
{

    public async Task<Pokemon> GetDailyPokemonAsync()
    {
        var pokemonCount = await context.Pokemons.CountAsync(p => p.Id < 10000);
        var seed = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
        var rng = new Random(seed);
        var index = rng.Next(1, pokemonCount);

        var daily = await context.Pokemons
            .Include(p => p.Habitat)
            .Include(p => p.Color)
            .Include(p => p.PokemonElementTypes)
                .ThenInclude(pet => pet.ElementType)
            .FirstOrDefaultAsync(p => p.Id == index)
            ?? throw new Exception("Daily Pokemon not Found");

        return daily;
    }

}
