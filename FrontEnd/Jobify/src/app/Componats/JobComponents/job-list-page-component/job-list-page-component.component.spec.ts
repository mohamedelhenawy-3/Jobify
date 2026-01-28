import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobListPageComponentComponent } from './job-list-page-component.component';

describe('JobListPageComponentComponent', () => {
  let component: JobListPageComponentComponent;
  let fixture: ComponentFixture<JobListPageComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobListPageComponentComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(JobListPageComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
