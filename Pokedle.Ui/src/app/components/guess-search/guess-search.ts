import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { debounceTime, switchMap } from 'rxjs/operators';
import { GameService } from '../../game.service';
import { PokemonService } from '../../pokemon.service';

@Component({
  selector: 'app-guess-search',
  imports: [FormsModule],
  templateUrl: './guess-search.html',
  styleUrl: './guess-search.css',
})
export class GuessSearch implements OnInit, OnDestroy {
  pokemonService = inject(PokemonService);
  gameService = inject(GameService);

  searchText = '';
  searchResults = signal<any[]>([]);
  showDropdown = signal<boolean>(false);

  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  ngOnInit() {
    this.searchSubject
      .pipe(
        debounceTime(300),
        switchMap((name) => this.pokemonService.searchPokemon(name)),
        takeUntil(this.destroy$),
      )
      .subscribe({
        next: (response: any) => {
          const alreadyGuessed = this.gameService.guesses().map((g: any) => g.guessPokemon);
          const filtered = response.data.allPokemon.filter(
            (pokemon: any) => !alreadyGuessed.includes(pokemon.name),
          );
          this.searchResults.set(filtered);
          this.showDropdown.set(filtered.length > 0);
        },
        error: (err) => console.error('Search error:', err),
      });
  }

  onSearch() {
    if (this.searchText.length < 2) {
      this.showDropdown.set(false);
      return;
    }
    this.searchSubject.next(this.searchText);
  }

  onSelect(name: string) {
    this.gameService.submitGuess(name);
    this.searchText = '';
    this.showDropdown.set(false);
    this.searchResults.set([]);
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
