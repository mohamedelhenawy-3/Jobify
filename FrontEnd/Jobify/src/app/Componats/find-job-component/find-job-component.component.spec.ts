import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FindJobComponentComponent } from './find-job-component.component';

describe('FindJobComponentComponent', () => {
  let component: FindJobComponentComponent;
  let fixture: ComponentFixture<FindJobComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FindJobComponentComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(FindJobComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
