import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuessSearch } from './guess-search';

describe('GuessSearch', () => {
  let component: GuessSearch;
  let fixture: ComponentFixture<GuessSearch>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuessSearch],
    }).compileComponents();

    fixture = TestBed.createComponent(GuessSearch);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
