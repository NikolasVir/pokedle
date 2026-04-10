import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { DAILY_POKEMON_ID_QUERY, SEARCH_POKEMON_QUERY } from './../graphql/queries';
import { GUESS_MUTATION } from './../graphql/mutations';
import { DAILY_POKEMON_SUBSCRIPTION } from '../graphql/subscriptions';
import { createClient } from 'graphql-ws';
import { Observable } from 'rxjs';

const API_URL = 'http://localhost:5065/graphql';
const WS_URL = 'ws://localhost:5065/graphql';

@Injectable({
  providedIn: 'root',
})
export class PokemonService {
  http = inject(HttpClient);

  private wsClient = createClient({ url: WS_URL });

  getDailyPokemonId() {
    return this.http.post(API_URL, { query: DAILY_POKEMON_ID_QUERY });
  }

  searchPokemon(name: string) {
    return this.http.post(API_URL, { query: SEARCH_POKEMON_QUERY(name) });
  }

  guess(pokemonName: string) {
    return this.http.post(API_URL, { query: GUESS_MUTATION(pokemonName) });
  }

  subscribeToGuesses(): Observable<string> {
    return new Observable((subscriber) => {
      const unsubscribe = this.wsClient.subscribe(
        { query: DAILY_POKEMON_SUBSCRIPTION },
        {
          next: (data: any) => subscriber.next(data.data?.onGuessMade),
          error: (err) => subscriber.error(err),
          complete: () => subscriber.complete(),
        },
      );
      return () => unsubscribe();
    });
  }
}
