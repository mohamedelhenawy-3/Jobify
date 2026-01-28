import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobspagesComponent } from './jobspages.component';

describe('JobspagesComponent', () => {
  let component: JobspagesComponent;
  let fixture: ComponentFixture<JobspagesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobspagesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(JobspagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
