using Microsoft.EntityFrameworkCore;
using Pokedle.Api.Infrastructure;
using PokeApiNet;
using Pokedle.Api.Infrastructure.Seeding;
using Pokedle.Api.GraphQL;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Pokedle.Api.Services;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' not found.");

    var jwtKey = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT key not configured.");

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    builder.Services.AddSerilog(Log.Logger);

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => o.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        });
    builder.Services.AddAuthorization();

    builder.Services.AddDbContext<PokedleContext>(options =>
        options.UseNpgsql(connectionString)
               .UseSnakeCaseNamingConvention());

    builder.Services.AddSingleton<PokeApiClient>();
    builder.Services.AddScoped<PokeApiSeeder>();
    builder.Services.AddScoped<DailyPokemonService>();

    builder.Services
        .AddGraphQLServer()
        .AddAuthorization()
        .AddInMemorySubscriptions()
        .AddQueryType<Query>()
        .AddMutationType<Mutation>()
        .AddSubscriptionType<Subscription>()
        .AddFiltering()
        .AddSorting();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? throw new InvalidOperationException("AllowedOrigins not configured.");

            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    var app = builder.Build();

    app.UseCors("AllowFrontend");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSerilogRequestLogging();

    app.UseWebSockets();
    app.MapGraphQL();

    if (args.Contains("--seed"))
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<PokeApiSeeder>();
        await seeder.SeedAsync();
        return;
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}