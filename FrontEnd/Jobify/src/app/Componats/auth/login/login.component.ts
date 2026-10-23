import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthserviceService } from '../../../Services/AuthService/authservice.service';
import { CommonModule } from '@angular/common';
import { OnboardingService } from '../../../Services/EmployerService/OnBoarding/onboarding.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule,RouterLink,CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginform!:FormGroup;
    constructor(private fb:FormBuilder,private authservice:AuthserviceService,private router:Router,private onboardingService: OnboardingService,){
     this.loginform = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
    }
  login() {
    if (this.loginform.invalid) {
      this.loginform.markAllAsTouched();
      return;
    }
this.authservice.login(this.loginform.value).subscribe({
      next: () => {
        const role = this.authservice.getUserRole(); 

        // 2. Handle routing based on Role
        if (role === 'Employer') {
          this.handleEmployerNavigation();
        } else if (role === 'Candidate' || role === 'JobSeeker') {
          this.router.navigate(['Jobify/home']);
        } else {
          this.router.navigate(['Jobify/home']); // Default fallback
        }
      },
      error: () => {
        alert('Email or Password Invalid');
      }
    });
  }
  private handleEmployerNavigation(): void {
    this.onboardingService.getOnboardingStatus().subscribe({
      next: (status) => {
        // Case 1: Employer has no company and hasn't requested to join one
        if (!status.hasCompany && !status.hasPendingJoinRequest) {
          this.router.navigate(['employer/onboarding']);
        } 
        // Case 2: Company verification or Join request is still pending
        else if (status.verificationStatus === 'Pending' || status.hasPendingJoinRequest) {
          this.router.navigate(['employer/pending-approval']);
        } 
        // Case 3: Verified employer with an active company
        else {
          this.router.navigate(['Jobify/home']);
        }
      },
      error: (err) => {
        console.error('Failed to get onboarding status:', err);
        this.router.navigate(['Jobify/home']);
      }
    });
  }
}
