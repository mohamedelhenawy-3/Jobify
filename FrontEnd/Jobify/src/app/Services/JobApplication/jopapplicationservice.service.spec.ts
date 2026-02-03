import { TestBed } from '@angular/core/testing';
import { JobApplicationService } from './jopapplicationservice.service';


describe('JopapplicationserviceService', () => {
  let service: JobApplicationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JobApplicationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
