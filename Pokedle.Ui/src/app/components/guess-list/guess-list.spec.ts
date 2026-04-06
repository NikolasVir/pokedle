import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuessList } from './guess-list';

describe('GuessList', () => {
  let component: GuessList;
  let fixture: ComponentFixture<GuessList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuessList],
    }).compileComponents();

    fixture = TestBed.createComponent(GuessList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
