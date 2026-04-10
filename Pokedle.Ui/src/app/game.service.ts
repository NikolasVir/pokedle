import { inject, Injectable, OnDestroy, signal } from '@angular/core';
import { Subscription } from 'rxjs';
import { PokemonService } from './pokemon.service';

@Injectable({
  providedIn: 'root',
})
export class GameService implements OnDestroy {
  private pokemonService = inject(PokemonService);
  private guessSub: Subscription;

  guesses = signal<Array<any>>([]);
  recentGuesses = signal<string[]>([]);
  isWon = signal<boolean>(false);

  constructor() {
    this.guessSub = this.pokemonService.subscribeToGuesses().subscribe({
      next: (name: string) => {
        this.recentGuesses.update((current) => [name, ...current]);
      },
      error: (err) => console.error('Subscription error:', err),
    });
  }

  ngOnDestroy() {
    this.guessSub.unsubscribe();
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
