import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JutsuForm } from './jutsu-form';

describe('JutsuForm', () => {
  let component: JutsuForm;
  let fixture: ComponentFixture<JutsuForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JutsuForm],
    }).compileComponents();

    fixture = TestBed.createComponent(JutsuForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
