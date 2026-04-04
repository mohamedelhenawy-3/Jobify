import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CompanySearchedComponent } from './company-searched.component';

describe('CompanySearchedComponent', () => {
  let component: CompanySearchedComponent;
  let fixture: ComponentFixture<CompanySearchedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompanySearchedComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CompanySearchedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
