import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnaithorizedComponent } from './unaithorized.component';

describe('UnaithorizedComponent', () => {
  let component: UnaithorizedComponent;
  let fixture: ComponentFixture<UnaithorizedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UnaithorizedComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UnaithorizedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
