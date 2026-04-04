import { TestBed } from '@angular/core/testing';

import { SearchsystemService } from './searchsystem.service';

describe('SearchsystemService', () => {
  let service: SearchsystemService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SearchsystemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
