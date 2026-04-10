import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RevealPanel } from './reveal-panel';

describe('RevealPanel', () => {
  let component: RevealPanel;
  let fixture: ComponentFixture<RevealPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RevealPanel],
    }).compileComponents();

    fixture = TestBed.createComponent(RevealPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
