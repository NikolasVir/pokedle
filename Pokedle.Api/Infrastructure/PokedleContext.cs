using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Domain;
using System.Reflection;

namespace Pokedle.Api.Infrastructure;

public class PokedleContext(DbContextOptions<PokedleContext> options)
    : DbContext(options)
{
    // Exclude alternate forms and regional variants (IDs >= 10000)
    public const int MaxBasePokemonId = 10000;

    public DbSet<Pokemon> Pokemons { get; set; }
    public DbSet<Habitat> Habitats { get; set; }
    public DbSet<PokemonColor> Colors { get; set; }
    public DbSet<PokemonElementType> PokemonElementTypes { get; set; }
    public DbSet<ElementType> ElementTypes { get; set; }
    public DbSet<GuessHistory> GuessHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}