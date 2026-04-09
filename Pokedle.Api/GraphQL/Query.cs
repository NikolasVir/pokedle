using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Domain;
using Pokedle.Api.Infrastructure;

namespace Pokedle.Api.GraphQL;

public class Query
{

    [Authorize]
    public async Task<Pokemon> RevealDailyPokemon([Service] PokedleContext context)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var index = today.DayNumber % await context.Pokemons.CountAsync(p => p.Id < 10000);

        return await context.Pokemons
            .Where(p => p.Id < 10000)
            .OrderBy(p => p.Id)
            .Skip(index)
            .Include(p => p.Habitat)
            .Include(p => p.Color)
            .Include(p => p.PokemonElementTypes)
                .ThenInclude(pet => pet.ElementType)
            .FirstAsync();
    }

    [UseFiltering]
    [UseSorting]
    public IQueryable<Pokemon> GetAllPokemon([Service] PokedleContext context) =>
        context.Pokemons
            .Where(p => p.Id < 10000)
            .Include(p => p.Habitat)
            .Include(p => p.Color)
            .Include(p => p.PokemonElementTypes)
                .ThenInclude(pet => pet.ElementType)
            .OrderBy(p => p.Id);

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