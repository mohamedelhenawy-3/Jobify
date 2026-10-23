import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SimilarJobsComponent } from './similar-jobs.component';

describe('SimilarJobsComponent', () => {
  let component: SimilarJobsComponent;
  let fixture: ComponentFixture<SimilarJobsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SimilarJobsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SimilarJobsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
