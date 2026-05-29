import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShinobiForm } from './shinobi-form';

describe('ShinobiForm', () => {
  let component: ShinobiForm;
  let fixture: ComponentFixture<ShinobiForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShinobiForm],
    }).compileComponents();

    fixture = TestBed.createComponent(ShinobiForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
