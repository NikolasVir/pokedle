import { Component, inject } from '@angular/core';
import { GameService } from '../../game.service';

@Component({
  selector: 'app-guess-list',
  imports: [],
  templateUrl: './guess-list.html',
  styleUrl: './guess-list.css',
})
export class GuessList {
  gameService = inject(GameService);

  getHint(hint: string): string {
    const map: Record<string, string> = {
      CORRECT: '✅',
      WRONG: '❌',
      HIGHER: '⬆️',
      LOWER: '⬇️',
    };
    return map[hint] ?? '—';
  }
}
