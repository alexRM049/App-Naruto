import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VillageForm } from './village-form';

describe('VillageForm', () => {
  let component: VillageForm;
  let fixture: ComponentFixture<VillageForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VillageForm],
    }).compileComponents();

    fixture = TestBed.createComponent(VillageForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
