import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { companyOnboardingGuardGuard } from './company-onboarding-guard.guard';

describe('companyOnboardingGuardGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => companyOnboardingGuardGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
