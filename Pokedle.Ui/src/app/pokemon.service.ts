import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SEARCH_POKEMON_QUERY, GET_DAILY_POKEMON_QUERY } from './../graphql/queries';
import { GUESS_MUTATION, LOGIN_MUTATION } from './../graphql/mutations';
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

  searchPokemon(name: string) {
    return this.http.post(API_URL, { query: SEARCH_POKEMON_QUERY(name) });
  }

  guess(pokemonName: string) {
    return this.http.post(API_URL, { query: GUESS_MUTATION(pokemonName) });
  }

  login(username: string, password: string) {
    return this.http.post(API_URL, { query: LOGIN_MUTATION(username, password) });
  }

  getDailyPokemon(token: string) {
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    return this.http.post(API_URL, { query: GET_DAILY_POKEMON_QUERY }, { headers });
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
