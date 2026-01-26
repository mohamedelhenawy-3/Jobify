import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeregisterComponent } from './employeregister.component';

describe('EmployeregisterComponent', () => {
  let component: EmployeregisterComponent;
  let fixture: ComponentFixture<EmployeregisterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmployeregisterComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EmployeregisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
