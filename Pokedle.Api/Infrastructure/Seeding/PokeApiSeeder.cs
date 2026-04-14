using Microsoft.EntityFrameworkCore;
using PokeApiNet;
using Pokedle.Api.Domain;
using Pokemon = Pokedle.Api.Domain.Pokemon;
using PokemonColor = Pokedle.Api.Domain.PokemonColor;

namespace Pokedle.Api.Infrastructure.Seeding;

public class PokeApiSeeder(PokedleContext context, PokeApiClient pokeApi, ILogger<PokeApiSeeder> logger)
{
    public async Task SeedAsync()
    {
        if (await context.Pokemons.AnyAsync())
        {
            logger.LogWarning("Database already seeded, skipping");
            return;
        }

        logger.LogInformation("Fetching all pokemon from PokeAPI");

        var page = await pokeApi.GetNamedResourcePageAsync<PokeApiNet.Pokemon>(limit: PokedleContext.MaxBasePokemonId, offset: 0);

        foreach (var resource in page.Results)
        {
            logger.LogInformation("Seeding {PokemonName}", resource.Name);

            var apiPokemon = await pokeApi.GetResourceAsync<PokeApiNet.Pokemon>(resource.Name);
            var species = await pokeApi.GetResourceAsync(apiPokemon.Species);

            var habitatName = species.Habitat?.Name ?? "unknown";
            var habitat = await GetOrCreateHabitat(habitatName);

            var color = await GetOrCreateColor(species.Color.Name);
            var evoStage = await GetEvolutionStage(species);
            var generation = GetGeneration(species);

            var pokemon = new Pokemon
            {
                Id = apiPokemon.Id,
                Name = apiPokemon.Name,
                Generation = generation,
                EvolutionStage = evoStage,
                ColorId = color.Id,
                HabitatId = habitat.Id
            };

            context.Pokemons.Add(pokemon);
            await context.SaveChangesAsync();

            foreach (var typeSlot in apiPokemon.Types)
            {
                var elementType = await GetOrCreateElementType(typeSlot.Type.Name);
                context.PokemonElementTypes.Add(new PokemonElementType
                {
                    PokemonId = pokemon.Id,
                    ElementTypeId = elementType.Id,
                    Slot = typeSlot.Slot
                });
            }

            await context.SaveChangesAsync();
        }

        logger.LogInformation("Seeded {Count} Pokemon", page.Results.Count);
    }

    private async Task<PokemonColor> GetOrCreateColor(string name)
    {
        var existing = await context.Colors.FirstOrDefaultAsync(c => c.Name == name);
        if (existing != null) return existing;
        var color = new PokemonColor { Name = name };
        context.Colors.Add(color);
        await context.SaveChangesAsync();
        return color;
    }

    private async Task<Habitat> GetOrCreateHabitat(string name)
    {
        var existing = await context.Habitats.FirstOrDefaultAsync(h => h.Name == name);
        if (existing != null) return existing;
        var habitat = new Habitat { Name = name };
        context.Habitats.Add(habitat);
        await context.SaveChangesAsync();
        return habitat;
    }

    private async Task<ElementType> GetOrCreateElementType(string name)
    {
        var existing = await context.ElementTypes.FirstOrDefaultAsync(e => e.Name == name);
        if (existing != null) return existing;
        var elementType = new ElementType { Name = name };
        context.ElementTypes.Add(elementType);
        await context.SaveChangesAsync();
        return elementType;
    }

    private async Task<int> GetEvolutionStage(PokemonSpecies species)
    {
        var chain = await pokeApi.GetResourceAsync(species.EvolutionChain);
        return FindStage(chain.Chain, species.Name, 1);
    }

    // Recursively walks the evolution chain tree to find the stage depth of the given species
    private static int FindStage(ChainLink link, string name, int stage)
    {
        if (link.Species.Name == name) return stage;
        foreach (var next in link.EvolvesTo)
        {
            var result = FindStage(next, name, stage + 1);
            if (result != -1) return result;
        }
        return -1; // not found
    }

    private static int GetGeneration(PokemonSpecies species)
    {
        var url = species.Generation.Url.TrimEnd('/');
        return int.Parse(url.Split('/').Last());
    }
}