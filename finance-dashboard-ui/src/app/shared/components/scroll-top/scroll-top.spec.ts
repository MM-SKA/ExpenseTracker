import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScrollTopComponent } from './scroll-top';

describe('ScrollTopComponent', () => {
  let component: ScrollTopComponent;
  let fixture: ComponentFixture<ScrollTopComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScrollTopComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ScrollTopComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
