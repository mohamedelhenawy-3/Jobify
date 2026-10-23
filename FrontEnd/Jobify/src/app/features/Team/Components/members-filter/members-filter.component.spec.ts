import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MembersFilterComponent } from './members-filter.component';

describe('MembersFilterComponent', () => {
  let component: MembersFilterComponent;
  let fixture: ComponentFixture<MembersFilterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MembersFilterComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MembersFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
