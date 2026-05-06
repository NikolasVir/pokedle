namespace Pokedle.Api.GraphQL.Types;

/// <summary>JWT token returned after successful login.</summary>
/// <param name="Token">The signed JWT.</param>
public record LoginResult(string Token);
