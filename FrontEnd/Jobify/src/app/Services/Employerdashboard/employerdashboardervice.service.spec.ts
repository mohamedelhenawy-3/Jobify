import { TestBed } from '@angular/core/testing';

import { EmployerdashboarderviceService } from './employerdashboardervice.service';

describe('EmployerdashboarderviceService', () => {
  let service: EmployerdashboarderviceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmployerdashboarderviceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
