import { computed, inject, Injectable, signal } from '@angular/core';
import { PokemonService } from './pokemon.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private pokemonService = inject(PokemonService);

  token = signal<string | null>(localStorage.getItem('token'));
  isLoggedIn = computed(() => this.token() !== null);

  login(username: string, password: string) {
    return new Promise<void>((resolve, reject) => {
      this.pokemonService.login(username, password).subscribe({
        next: (response: any) => {
          const token = response.data?.login?.token;
          if (token) {
            this.token.set(token);
            localStorage.setItem('token', token);
            resolve();
          } else {
            reject(response.errors?.[0]?.message ?? 'Login failed');
          }
        },
        error: (err) => reject(err),
      });
    });
  }

  logout() {
    this.token.set(null);
    localStorage.removeItem('token');
  }
}
