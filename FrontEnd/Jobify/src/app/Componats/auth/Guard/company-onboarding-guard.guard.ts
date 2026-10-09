import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map, of } from 'rxjs';
import { OnboardingService } from '../../../Services/EmployerService/OnBoarding/onboarding.service';
import { AuthserviceService } from '../../../Services/AuthService/authservice.service';

export const companyOnboardingGuard: CanActivateFn = (route, state) => {
  const onboardingService = inject(OnboardingService);
  const authService = inject(AuthserviceService);
  const router = inject(Router);

  // 1. Check user role first
  const role = authService.getUserRole();

  // 2. If user is NOT an Employer, allow access immediately
  if (role !== 'Employer') {
    return of(true);
  }

  // 3. Perform onboarding status check for Employers
  return onboardingService.getOnboardingStatus().pipe(
    map(status => {
      // Case 1: No company & no join request -> Must select Create or Join
      if (!status.hasCompany && !status.hasPendingJoinRequest) {
        router.navigate(['employer/onboarding']);
        return false;
      }

      // Case 2: Company verification or Join request is pending -> Show waiting page
      if (
        (status.hasCompany && status.verificationStatus === 'Pending') ||
        (!status.hasCompany && status.hasPendingJoinRequest)
      ) {
        router.navigate(['employer/pending-approval']);
        return false;
      }

      // Case 3: Fully verified and approved -> Access granted to dashboard
      return true;
    })
  );
};