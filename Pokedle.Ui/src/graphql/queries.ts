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

export const GET_DAILY_POKEMON_QUERY = `
  query {
    dailyPokemon {
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
