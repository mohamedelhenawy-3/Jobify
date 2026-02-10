import { TestBed } from '@angular/core/testing';

import { DashboarderviceService } from './dashboardervice.service';

describe('DashboarderviceService', () => {
  let service: DashboarderviceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DashboarderviceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
