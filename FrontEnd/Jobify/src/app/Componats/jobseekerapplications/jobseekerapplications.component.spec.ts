import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobseekerapplicationsComponent } from './jobseekerapplications.component';

describe('JobseekerapplicationsComponent', () => {
  let component: JobseekerapplicationsComponent;
  let fixture: ComponentFixture<JobseekerapplicationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobseekerapplicationsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(JobseekerapplicationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
