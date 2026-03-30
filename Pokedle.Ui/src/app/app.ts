import { Component, signal } from '@angular/core';
import { Header } from './components/header/header';
import { GuessMessage } from './components/guess-message/guess-message';

@Component({
  selector: 'app-root',
  imports: [Header, GuessMessage],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Pokedle.Ui');
}
