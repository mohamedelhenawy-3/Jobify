import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobdashboardComponent } from './jobdashboard.component';

describe('JobdashboardComponent', () => {
  let component: JobdashboardComponent;
  let fixture: ComponentFixture<JobdashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobdashboardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(JobdashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
