import { Component, inject } from '@angular/core';
import { GameService } from '../../game.service';

@Component({
  selector: 'app-guess-message',
  imports: [],
  templateUrl: './guess-message.html',
  styleUrl: './guess-message.css',
})
export class GuessMessage {
  gameService = inject(GameService);
}
