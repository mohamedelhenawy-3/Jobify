import { TestBed } from '@angular/core/testing';

import { SavedjobsService } from './savedjobs.service';

describe('SavedjobsService', () => {
  let service: SavedjobsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SavedjobsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
