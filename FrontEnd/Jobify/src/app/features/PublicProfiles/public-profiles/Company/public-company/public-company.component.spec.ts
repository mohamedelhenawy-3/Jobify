import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicCompanyComponent } from './public-company.component';

describe('PublicCompanyComponent', () => {
  let component: PublicCompanyComponent;
  let fixture: ComponentFixture<PublicCompanyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PublicCompanyComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PublicCompanyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
