import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import {  OnboardingService } from '../../../Services/EmployerService/OnBoarding/onboarding.service';
import { Console } from 'console';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';


@Component({
  selector: 'app-createcompany',
  standalone: true,
  imports: [ReactiveFormsModule,FormsModule,CommonModule],
  templateUrl: './createcompany.component.html',
  styleUrl: './createcompany.component.css'
})
export class CreatecompanyComponent {

  isSaving: boolean = false;

  companyForm: FormGroup = this.fb.group({
    companyName: ['', [Validators.required, Validators.minLength(2)]],
    industry: ['', Validators.required],
    location: ['', Validators.required],
    companySize: ['', Validators.required],
    founded: ['', [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear())]],
    companyWebsite: ['', Validators.pattern(/^https?:\/\/.+\..+/)],
    description: ['']
  });

  constructor(
    private fb: FormBuilder,
    private onboardingservice: OnboardingService,private router:Router
  ) {}

  onSubmit(): void {
    if (this.companyForm.invalid) {
      this.companyForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;

    // Convert founded to string to match backend requirement ("founded": "string")
    const payload = {
      companyName: this.companyForm.value.companyName,
      companyWebsite: this.companyForm.value.companyWebsite,
      description: this.companyForm.value.description,
      location: this.companyForm.value.location,
      industry: this.companyForm.value.industry,
      companySize: this.companyForm.value.companySize,
      founded: String(this.companyForm.value.founded)
    };

    console.log('Sending Payload:', payload);

    this.onboardingservice.CreateCompany(payload).subscribe({
      next: (res) => {
        console.log('Company created successfully:', res);
        this.isSaving = false;
        // Redirect to pending approval route
        this.router.navigate(['employer/pending-approval']);
        
      },
      error: (err) => {
        console.error('Failed to create company:', err);
        this.isSaving = false;

      }
    });
  }
}
