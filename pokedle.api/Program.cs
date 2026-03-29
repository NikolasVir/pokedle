using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Infrastructure;
using PokeApiNet;
using Pokedle.Api.Infrastructure.Seeding;
using Pokedle.Api.GraphQL;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found.");

builder.Services.AddDbContext<PokedleContext>(options =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention());

builder.Services.AddSingleton<PokeApiClient>();
builder.Services.AddScoped<PokeApiSeeder>();
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGraphQL();

if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<PokeApiSeeder>();
    await seeder.SeedAsync();
    return;
}

app.Run();

