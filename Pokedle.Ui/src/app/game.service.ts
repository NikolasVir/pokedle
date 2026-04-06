import { inject, Injectable, signal } from '@angular/core';
import { PokemonService } from './pokemon.service';

@Injectable({
  providedIn: 'root',
})
export class GameService {
  private pokemonService = inject(PokemonService);

  dailyPokemonId = signal<number>(0);
  guesses = signal<Array<any>>([]);
  isWon = signal<boolean>(false);

  constructor() {
    this.pokemonService.getDailyPokemonId().subscribe({
      next: (response: any) => this.dailyPokemonId.set(response.data.dailyPokemonId),
      error: (err) => console.error('Error fetching daily Pokémon:', err),
    });
  }

  submitGuess(pokemonName: string) {
    this.pokemonService.guess(pokemonName).subscribe({
      next: (response: any) => {
        this.guesses.update((current) => [...current, response.data.guess]);
        if (response.data.guess.isCorrect) {
          this.isWon.set(true);
        }
        console.log('Guesses so far:', this.guesses());
      },
      error: (err) => console.error('Error fetching daily Pokémon:', err),
    });
  }
}
