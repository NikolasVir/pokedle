using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pokedle.Api.GraphQL.Types;
using Pokedle.Api.Infrastructure;


namespace Pokedle.Api.GraphQL;

public class Mutation()
{

    public LoginResult Login(
        string username,
        string password,
        IConfiguration configuration)
    {
        if (username != "ash" || password != "pikachu")
            throw new GraphQLException("Invalid credentials.");

        var key = configuration["Jwt:Key"]!;
        var token = new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(
                claims: [new Claim(ClaimTypes.Name, username)],
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256)
            ));

        return new LoginResult(token);
    }
    public async Task<GuessResult> Guess(string pokemonName,
     [Service] PokedleContext context,
      [Service] ILogger<Mutation> logger,
      [Service] ITopicEventSender sender)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var seed = today.DayNumber;
        var count = await context.Pokemons.CountAsync(p => p.Id < 10000);
        var index = seed % count;

        var daily = await context.Pokemons
            .Where(p => p.Id < 10000)
            .OrderBy(p => p.Id)
            .Skip(index)
            .Include(p => p.Habitat)
            .Include(p => p.Color)
            .Include(p => p.PokemonElementTypes)
                .ThenInclude(pet => pet.ElementType)
            .FirstAsync();

        var guess = await context.Pokemons
            .Where(p => p.Name == pokemonName.ToLower())
            .Include(p => p.Habitat)
            .Include(p => p.Color)
            .Include(p => p.PokemonElementTypes)
                .ThenInclude(pet => pet.ElementType)
            .FirstOrDefaultAsync()
            ?? throw new GraphQLException($"Pokémon '{pokemonName}' not found.");

        var guessTypes = guess.PokemonElementTypes
            .Select(t => ((int?)t.ElementTypeId, t.ElementType!.Name))
            .ToList();

        var dailyTypeIds = daily.PokemonElementTypes
            .Select(t => (int?)t.ElementTypeId)
            .ToList();

        while (guessTypes.Count < 2) guessTypes.Add((null, "none"));
        while (dailyTypeIds.Count < 2) dailyTypeIds.Add(null);

        TypeSlotHint GetSlotHint(int slot)
        {
            var (typeId, typeName) = guessTypes[slot];

            if (typeId == null)
                return new TypeSlotHint { TypeName = "none", Hint = dailyTypeIds[slot] == null ? Hint.Correct : Hint.Wrong };

            if (dailyTypeIds[slot] == typeId)
                return new TypeSlotHint { TypeName = typeName, Hint = Hint.Correct };

            if (dailyTypeIds.Contains(typeId))
                return new TypeSlotHint { TypeName = typeName, Hint = Hint.Partial };

            return new TypeSlotHint { TypeName = typeName, Hint = Hint.Wrong };
        }

        var result = new GuessResult
        {
            GuessPokemon = guess.Name,
            Generation = guess.Generation == daily.Generation ? Hint.Correct
                         : guess.Generation < daily.Generation ? Hint.Higher
                         : Hint.Lower,
            Habitat = guess.HabitatId == daily.HabitatId ? Hint.Correct : Hint.Wrong,
            Color = new ColorResult { ColorName = guess.Color!.Name, Hint = guess.ColorId == daily.ColorId ? Hint.Correct : Hint.Wrong },
            Types = new TypeHintResult { Slot1 = GetSlotHint(0), Slot2 = GetSlotHint(1) },
            IsCorrect = guess.Id == daily.Id
        };

        logger.LogInformation(
            "Guess result: {PokemonName} vs daily={DailyPokemonId} result={@Result}",
            guess.Name,
            daily.Id,
            result
        );

        await sender.SendAsync(nameof(Subscription.OnGuessMade), guess.Name);

        return result;
    }
}
