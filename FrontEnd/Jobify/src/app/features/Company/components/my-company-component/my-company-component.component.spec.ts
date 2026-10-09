import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyCompanyComponentComponent } from './my-company-component.component';

describe('MyCompanyComponentComponent', () => {
  let component: MyCompanyComponentComponent;
  let fixture: ComponentFixture<MyCompanyComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyCompanyComponentComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MyCompanyComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
