using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pokedle.Api.Domain;

namespace Pokedle.Api.Infrastructure.Configurations;

public class PokemonConfiguration : IEntityTypeConfiguration<Pokemon>
{
    public void Configure(EntityTypeBuilder<Pokemon> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Habitat)
               .WithMany()
               .HasForeignKey(p => p.HabitatId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Color)
               .WithMany()
               .HasForeignKey(p => p.ColorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}