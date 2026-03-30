import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuessMessage } from './guess-message';

describe('GuessMessage', () => {
  let component: GuessMessage;
  let fixture: ComponentFixture<GuessMessage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuessMessage],
    }).compileComponents();

    fixture = TestBed.createComponent(GuessMessage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
