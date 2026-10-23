import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicProfilesComponent } from './public-profiles.component';

describe('PublicProfilesComponent', () => {
  let component: PublicProfilesComponent;
  let fixture: ComponentFixture<PublicProfilesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PublicProfilesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PublicProfilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
