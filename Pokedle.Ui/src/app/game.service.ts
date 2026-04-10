import { inject, Injectable, signal } from '@angular/core';
import { PokemonService } from './pokemon.service';

@Injectable({
  providedIn: 'root',
})
export class GameService {
  private pokemonService = inject(PokemonService);

  guesses = signal<Array<any>>([]);
  recentGuesses = signal<string[]>([]);
  isWon = signal<boolean>(false);

  constructor() {
    this.pokemonService.subscribeToGuesses().subscribe({
      next: (name: string) => {
        this.recentGuesses.update((current) => [name, ...current]);
      },
      error: (err) => console.error('Subscription error:', err),
    });
  }

  submitGuess(pokemonName: string) {
    this.pokemonService.guess(pokemonName).subscribe({
      next: (response: any) => {
        this.guesses.update((current) => [...current, response.data.guess]);
        if (response.data.guess.isCorrect) {
          this.isWon.set(true);
        }
      },
      error: (err) => console.error('Error submitting guess:', err),
    });
  }
}
