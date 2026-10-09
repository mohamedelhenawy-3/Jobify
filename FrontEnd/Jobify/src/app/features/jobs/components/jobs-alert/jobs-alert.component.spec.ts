import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobsAlertComponent } from './jobs-alert.component';

describe('JobsAlertComponent', () => {
  let component: JobsAlertComponent;
  let fixture: ComponentFixture<JobsAlertComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobsAlertComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(JobsAlertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
