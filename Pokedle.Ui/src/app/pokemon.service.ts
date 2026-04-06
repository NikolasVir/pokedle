import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { DAILY_POKEMON_ID_QUERY, SEARCH_POKEMON_QUERY } from './../graphql/queries';
import { GUESS_MUTATION } from './../graphql/mutations';

const API_URL = 'http://localhost:5065/graphql';

@Injectable({
  providedIn: 'root',
})
export class PokemonService {
  http = inject(HttpClient);

  getDailyPokemonId() {
    return this.http.post(API_URL, { query: DAILY_POKEMON_ID_QUERY });
  }

  searchPokemon(name: string) {
    return this.http.post(API_URL, { query: SEARCH_POKEMON_QUERY(name) });
  }

  guess(pokemonName: string) {
    return this.http.post(API_URL, { query: GUESS_MUTATION(pokemonName) });
  }
}
