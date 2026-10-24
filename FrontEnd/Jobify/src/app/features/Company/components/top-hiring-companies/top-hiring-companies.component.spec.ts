import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopHiringCompaniesComponent } from './top-hiring-companies.component';

describe('TopHiringCompaniesComponent', () => {
  let component: TopHiringCompaniesComponent;
  let fixture: ComponentFixture<TopHiringCompaniesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TopHiringCompaniesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TopHiringCompaniesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
