using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=127.0.0.1;Port=5432;Database=pokedle;Username=pokedle;Password=pokedle";

builder.Services.AddDbContext<PokedleContext>(options =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
