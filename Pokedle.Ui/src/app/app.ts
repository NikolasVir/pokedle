import { Component, inject, signal } from '@angular/core';
import { Header } from './components/header/header';
import { GuessMessage } from './components/guess-message/guess-message';
import { PokemonService } from './pokemon.service';
import { GuessSearch } from './components/guess-search/guess-search';

@Component({
  selector: 'app-root',
  imports: [Header, GuessMessage, GuessSearch],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Pokedle.Ui');
  pokemonService = inject(PokemonService);

  ngOnInit() {
    this.pokemonService.getDailyPokemonId().subscribe({
      next: (data) => console.log('Response:', data),
      error: (err) => console.error('Error:', err),
    });
  }
}
