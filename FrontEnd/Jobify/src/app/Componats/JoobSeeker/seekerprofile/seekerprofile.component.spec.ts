import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeekerprofileComponent } from './seekerprofile.component';

describe('SeekerprofileComponent', () => {
  let component: SeekerprofileComponent;
  let fixture: ComponentFixture<SeekerprofileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeekerprofileComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SeekerprofileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
