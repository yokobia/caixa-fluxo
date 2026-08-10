import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TesteCarga } from './teste-carga';

describe('TesteCarga', () => {
  let component: TesteCarga;
  let fixture: ComponentFixture<TesteCarga>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TesteCarga],
    }).compileComponents();

    fixture = TestBed.createComponent(TesteCarga);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
