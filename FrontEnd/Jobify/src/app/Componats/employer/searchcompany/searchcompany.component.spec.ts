import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchcompanyComponent } from './searchcompany.component';

describe('SearchcompanyComponent', () => {
  let component: SearchcompanyComponent;
  let fixture: ComponentFixture<SearchcompanyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchcompanyComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SearchcompanyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
