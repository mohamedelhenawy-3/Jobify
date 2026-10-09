import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatecompanyComponent } from './createcompany.component';

describe('CreatecompanyComponent', () => {
  let component: CreatecompanyComponent;
  let fixture: ComponentFixture<CreatecompanyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreatecompanyComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreatecompanyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
