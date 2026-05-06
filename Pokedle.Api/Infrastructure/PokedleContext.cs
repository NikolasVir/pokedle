using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Domain;
using System.Reflection;

namespace Pokedle.Api.Infrastructure;

/// <summary>EF Core database context for Pokedle.</summary>
public class PokedleContext(DbContextOptions<PokedleContext> options)
    : DbContext(options)
{
    /// <summary>Threshold to exclude alternate forms and regional variants (IDs >= 10000).</summary>
    public const int MaxBasePokemonId = 10000;

    /// <summary>Pokémon table.</summary>
    public DbSet<Pokemon> Pokemons { get; set; }
    /// <summary>Habitats table.</summary>
    public DbSet<Habitat> Habitats { get; set; }
    /// <summary>Colors table.</summary>
    public DbSet<PokemonColor> Colors { get; set; }
    /// <summary>Pokémon-element join table.</summary>
    public DbSet<PokemonElementType> PokemonElementTypes { get; set; }
    /// <summary>Element types table.</summary>
    public DbSet<ElementType> ElementTypes { get; set; }
    /// <summary>Guess history table.</summary>
    public DbSet<GuessHistory> GuessHistories { get; set; }

    /// <summary>Applies entity configurations from the assembly.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
