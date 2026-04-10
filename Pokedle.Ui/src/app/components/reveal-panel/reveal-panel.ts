import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../auth.service';
import { PokemonService } from '../../pokemon.service';

type ViewState = 'hidden' | 'form' | 'result';

@Component({
  selector: 'app-reveal-panel',
  imports: [FormsModule],
  templateUrl: './reveal-panel.html',
  styleUrl: './reveal-panel.css',
})
export class RevealPanel {
  private authService = inject(AuthService);
  private pokemonService = inject(PokemonService);

  viewState = signal<ViewState>('hidden');
  username = signal('');
  password = signal('');
  error = signal<string | null>(null);
  dailyPokemon = signal<any>(null);

  showForm() {
    this.viewState.set('form');
  }

  async submit() {
    this.error.set(null);
    try {
      await this.authService.login(this.username(), this.password());
      this.pokemonService.getDailyPokemon(this.authService.token()!).subscribe({
        next: (response: any) => {
          this.dailyPokemon.set(response.data?.dailyPokemon);
          this.viewState.set('result');
        },
        error: () => this.error.set('Failed to fetch daily Pokémon.'),
      });
    } catch (err: any) {
      this.error.set(typeof err === 'string' ? err : 'Invalid credentials.');
    }
  }
}
