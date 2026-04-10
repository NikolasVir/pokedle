import { Component, signal } from '@angular/core';
import { Header } from './components/header/header';
import { GuessMessage } from './components/guess-message/guess-message';
import { GuessSearch } from './components/guess-search/guess-search';
import { GuessList } from './components/guess-list/guess-list';
import { RecentGuessesFeed } from './components/recent-guesses-feed/recent-guesses-feed';

@Component({
  selector: 'app-root',
  imports: [Header, GuessMessage, GuessSearch, GuessList, RecentGuessesFeed],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Pokedle.Ui');
}
