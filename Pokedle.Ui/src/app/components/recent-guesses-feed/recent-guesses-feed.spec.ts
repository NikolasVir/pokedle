import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecentGuessesFeed } from './recent-guesses-feed';

describe('RecentGuessesFeed', () => {
  let component: RecentGuessesFeed;
  let fixture: ComponentFixture<RecentGuessesFeed>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecentGuessesFeed],
    }).compileComponents();

    fixture = TestBed.createComponent(RecentGuessesFeed);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
