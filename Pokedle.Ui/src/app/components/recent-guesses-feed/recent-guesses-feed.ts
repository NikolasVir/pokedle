import { Component, inject } from '@angular/core';
import { GameService } from '../../game.service';

@Component({
  selector: 'app-recent-guesses-feed',
  imports: [],
  templateUrl: './recent-guesses-feed.html',
  styleUrl: './recent-guesses-feed.css',
})
export class RecentGuessesFeed {
  gameService = inject(GameService);
}
