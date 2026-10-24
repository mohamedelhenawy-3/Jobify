import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinSectionComponent } from './join-section.component';

describe('JoinSectionComponent', () => {
  let component: JoinSectionComponent;
  let fixture: ComponentFixture<JoinSectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JoinSectionComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(JoinSectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
