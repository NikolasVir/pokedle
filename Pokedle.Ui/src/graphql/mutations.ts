export const GUESS_MUTATION = (pokemonName: string) => `
  mutation {
    guess(pokemonName: "${pokemonName}") {
      guessPokemon
      generation
      habitat
      color
      types {
        slot1 { typeName hint }
        slot2 { typeName hint }
      }
      isCorrect
    }
  }
`;
