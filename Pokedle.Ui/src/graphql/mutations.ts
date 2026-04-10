export const GUESS_MUTATION = (pokemonName: string) => `
  mutation {
    guess(pokemonName: "${pokemonName}") {
      guessPokemon
      generation
      habitat
      color {
        colorName
        hint
      }
      types {
        slot1 { typeName hint }
        slot2 { typeName hint }
      }
      isCorrect
    }
  }
`;

export const LOGIN_MUTATION = (username: string, password: string) => `
  mutation {
    login(username: "${username}", password: "${password}") {
      token
    }
  }
`;
