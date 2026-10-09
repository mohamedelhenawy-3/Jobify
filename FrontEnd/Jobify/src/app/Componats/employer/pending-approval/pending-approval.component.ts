import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { OnboardingService } from '../../../Services/EmployerService/OnBoarding/onboarding.service';
import { AuthserviceService } from '../../../Services/AuthService/authservice.service';
import { EmployerOnboardingStatus } from '../../../Models/OnBoarding/OnboardingStatus';

@Component({
  selector: 'app-pending-approval',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pending-approval.component.html',
  styleUrl: './pending-approval.component.css'
})
export class PendingApprovalComponent implements OnInit {
  status: EmployerOnboardingStatus | null = null;
  isRefreshing: boolean = false;

  constructor(
    private onboardingService: OnboardingService,
    private authService: AuthserviceService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.refreshStatus();
  }

  refreshStatus(): void {
    this.isRefreshing = true;

    this.onboardingService.getOnboardingStatus().subscribe({
      next: (res: EmployerOnboardingStatus) => {
        this.status = res;
        this.isRefreshing = false;

        // 1. SCENARIO APPROVED:
        // Either Company Owner is verified OR Employee join request was approved (now has company)
        const isApprovedOwner = res.hasCompany && res.verificationStatus === 'Approved';
        const isApprovedEmployee = res.hasCompany && !res.hasPendingJoinRequest;

        if (isApprovedOwner || isApprovedEmployee) {
          // Sync JWT/Session to pick up updated CompanyId claims before going to Dashboard
          this.authService.refreshToken().subscribe({
            next: () => this.router.navigate(['Jobify/home']),
            error: () => this.router.navigate(['Jobify/home'])
          });
          return;
        }

        // 2. SCENARIO REJECTED:
        // No company linked AND no active pending join request -> Request was declined by owner/admin
        if (!res.hasCompany && !res.hasPendingJoinRequest && res.verificationStatus === 'Rejected') {
          this.router.navigate(['employer/onboarding'], {
            queryParams: { status: 'rejected' }
          });
        }
      },
      error: (err) => {
        console.error('Failed to fetch onboarding status:', err);
        this.isRefreshing = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}