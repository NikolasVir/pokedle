using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pokedle.Api.Domain;

namespace Pokedle.Api.Infrastructure.Configurations;

/// <summary>EF Core configuration for the PokemonElementType join entity.</summary>
public class PokemonElementTypeConfiguration : IEntityTypeConfiguration<PokemonElementType>
{
    /// <summary>Configures relationships, keys, and unique index.</summary>
    public void Configure(EntityTypeBuilder<PokemonElementType> builder)
    {
        builder.HasKey(pet => pet.Id);

        builder.HasOne(pet => pet.Pokemon)
               .WithMany(p => p.PokemonElementTypes)
               .HasForeignKey(pet => pet.PokemonId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pet => pet.ElementType)
               .WithMany()
               .HasForeignKey(pet => pet.ElementTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pet => new { pet.PokemonId, pet.Slot })
               .IsUnique();
    }
}