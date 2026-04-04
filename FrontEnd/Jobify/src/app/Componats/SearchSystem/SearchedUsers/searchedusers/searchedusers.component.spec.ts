import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchedusersComponent } from './searchedusers.component';

describe('SearchedusersComponent', () => {
  let component: SearchedusersComponent;
  let fixture: ComponentFixture<SearchedusersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchedusersComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SearchedusersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
