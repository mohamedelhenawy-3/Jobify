import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { OnboardingService } from '../../../Services/EmployerService/OnBoarding/onboarding.service';
import { CompanySearchDto } from '../../../Models/OnBoarding/CompanySearch';

@Component({
  selector: 'app-join-company',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './join-company.component.html',
  styleUrl: './join-company.component.css'
})
export class JoinCompanyComponent implements OnInit {
  searchTerm: string = '';
  companies: CompanySearchDto[] = [];
  selectedCompanyId: number | null = null;
  
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  errorMessage: string = '';

  constructor(
    private onboardingService: OnboardingService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchCompanies();
  }

  fetchCompanies(): void {
    this.isLoading = true;
    this.onboardingService.searchCompanies(this.searchTerm).subscribe({
      next: (results: CompanySearchDto[]) => {
        this.companies = results;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching companies:', err);
        this.errorMessage = 'Failed to load companies. Please try again.';
        this.isLoading = false;
      }
    });
  }

  onSearchChange(): void {
    this.fetchCompanies();
  }

  selectCompany(companyId: number): void {
    this.selectedCompanyId = companyId;
  }

  submitJoinRequest(): void {
    if (!this.selectedCompanyId) return;

    this.isSubmitting = true;
    this.errorMessage = '';

    this.onboardingService.joinCompany(this.selectedCompanyId).subscribe({
      next: (res) => {
        this.isSubmitting = false;
        // Redirect to pending approval page upon successful submission
        this.router.navigate(['employer/pending-approval']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err.error?.message || err.error || 'Failed to submit join request.';
      }
    });
  }
}