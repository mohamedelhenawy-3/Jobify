import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobseekerdashboardComponent } from './jobseekerdashboard.component';

describe('JobseekerdashboardComponent', () => {
  let component: JobseekerdashboardComponent;
  let fixture: ComponentFixture<JobseekerdashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobseekerdashboardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(JobseekerdashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
