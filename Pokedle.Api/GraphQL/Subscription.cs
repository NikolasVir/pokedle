namespace Pokedle.Api.GraphQL;

/// <summary>GraphQL subscription root.</summary>
public class Subscription
{
    /// <summary>Notifies subscribers when a guess is made.</summary>
    [Subscribe]
    [Topic]
    public string OnGuessMade([EventMessage] string pokemonName) => pokemonName;
}
