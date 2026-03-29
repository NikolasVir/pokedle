using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Domain;
using Pokedle.Api.Infrastructure;

namespace Pokedle.Api.GraphQL;

public class Query
{
    public async Task<List<Pokemon>> GetAllPokemon([Service] PokedleContext context) =>
        await context.Pokemons
            .Where(p => p.Id < 10000)
            .Include(p => p.Habitat)
            .Include(p => p.Color)
            .Include(p => p.PokemonElementTypes)
                .ThenInclude(pet => pet.ElementType)
            .OrderBy(p => p.Id)
            .ToListAsync();

    public async Task<int> GetDailyPokemonId([Service] PokedleContext context)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var seed = today.DayNumber;
        var count = await context.Pokemons.CountAsync(p => p.Id < 10000);
        var index = seed % count;
        return await context.Pokemons
            .Where(p => p.Id < 10000)
            .OrderBy(p => p.Id)
            .Skip(index)
            .Select(p => p.Id)
            .FirstAsync();
    }
}