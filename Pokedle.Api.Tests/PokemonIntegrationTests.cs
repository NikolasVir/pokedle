using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Pokedle.Api.Infrastructure;

namespace Pokedle.Api.Tests;

public class PokemonIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PokemonIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    private void SeedDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PokedleContext>();
        if (!context.Pokemons.Any())
        {
            context.Pokemons.Add(new Pokedle.Api.Domain.Pokemon { Name = "Pikachu" });
            context.SaveChanges();
        }
    }

    [Fact]
    public void DbIsSeeded()
    {
        SeedDatabase();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PokedleContext>();
        context.Pokemons.Should().Contain(p => p.Name == "Pikachu");
    }

    [Fact]
    public async Task GraphQL_GetAllPokemon_Returns_OK()
    {
        SeedDatabase();
        var client = _factory.CreateClient();
        var queryJson = @"{""query"":""{ allPokemon { name } }""}";
        using var content = new StringContent(queryJson, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/graphql", content);
        var body = await response.Content.ReadAsStringAsync();
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Xunit.Sdk.XunitException($"GraphQL request failed ({(int)response.StatusCode}): {body}");
        }
        body.Should().Contain("\"data\":");
        body.Should().Contain("\"allPokemon\"");
    }
}
