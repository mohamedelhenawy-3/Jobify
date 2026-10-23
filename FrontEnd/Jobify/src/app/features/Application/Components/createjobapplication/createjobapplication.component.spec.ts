import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatejobapplicationComponent } from './createjobapplication.component';

describe('CreatejobapplicationComponent', () => {
  let component: CreatejobapplicationComponent;
  let fixture: ComponentFixture<CreatejobapplicationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreatejobapplicationComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreatejobapplicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
