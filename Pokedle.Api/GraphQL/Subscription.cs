namespace Pokedle.Api.GraphQL;

public class Subscription
{
    [Subscribe]
    [Topic]
    public string OnGuessMade([EventMessage] string pokemonName) => pokemonName;
}