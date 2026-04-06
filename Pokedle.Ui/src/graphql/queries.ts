export const DAILY_POKEMON_ID_QUERY = `
  query {
    dailyPokemonId
  }
`;

export const SEARCH_POKEMON_QUERY = (name: string) => `
  query {
    allPokemon(where: { name: { startsWith: "${name}" } }) {
      name
      generation
      habitat { name }
      color { name }
      pokemonElementTypes {
        elementType { name }
      }
    }
  }
`;
