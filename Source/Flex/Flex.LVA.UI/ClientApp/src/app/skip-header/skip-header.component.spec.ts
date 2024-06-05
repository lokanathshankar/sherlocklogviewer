import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SkipHeaderComponent } from './skip-header.component';

describe('SkipHeaderComponent', () => {
  let component: SkipHeaderComponent;
  let fixture: ComponentFixture<SkipHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SkipHeaderComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SkipHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
