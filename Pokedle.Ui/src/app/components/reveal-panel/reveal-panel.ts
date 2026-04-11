import { Component, inject, signal } from '@angular/core';
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
    if (this.authService.isLoggedIn()) {
      this.fetchDailyPokemon();
    } else {
      this.viewState.set('form');
    }
  }

  fetchDailyPokemon() {
    this.pokemonService.getDailyPokemon(this.authService.token()!).subscribe({
      next: (response: any) => {
        const authError = response.errors?.find(
          (e: any) => e.extensions?.code === 'AUTH_NOT_AUTHENTICATED',
        );

        if (authError) {
          this.authService.logout();
          this.viewState.set('form');
          return;
        }

        this.dailyPokemon.set(response.data?.dailyPokemon);
        this.viewState.set('result');
      },
      error: () => this.error.set('Failed to fetch daily Pokémon.'),
    });
  }

  async submit() {
    this.error.set(null);
    try {
      await this.authService.login(this.username(), this.password());
      this.fetchDailyPokemon();
    } catch (err: any) {
      this.error.set(typeof err === 'string' ? err : 'Invalid credentials.');
    }
  }
}
