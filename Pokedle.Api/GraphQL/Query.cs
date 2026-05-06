using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Domain;
using Pokedle.Api.Infrastructure;
using Pokedle.Api.Services;

namespace Pokedle.Api.GraphQL;

/// <summary>GraphQL query root.</summary>
public class Query
{

    /// <summary>Returns today's Pokémon (requires auth).</summary>
    [Authorize]
    public async Task<Pokemon> GetDailyPokemon([Service] DailyPokemonService dailyPokemonService)
    {
        return await dailyPokemonService.GetDailyPokemonAsync();
    }

    /// <summary>Returns all base-form Pokémon with filtering and sorting.</summary>
    [UseFiltering]
    [UseSorting]
    public IQueryable<Pokemon> GetAllPokemon([Service] PokedleContext context) =>
        context.Pokemons
            .Where(p => p.Id < PokedleContext.MaxBasePokemonId)
            .Include(p => p.Habitat)
            .Include(p => p.Color)
            .Include(p => p.PokemonElementTypes)
                .ThenInclude(pet => pet.ElementType)
            .OrderBy(p => p.Id);
}
