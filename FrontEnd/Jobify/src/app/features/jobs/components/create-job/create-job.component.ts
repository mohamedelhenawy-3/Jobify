import { Component } from '@angular/core';
import { JobService } from '../../Service/job.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ExperienceLevel, JobType } from '../update-job/update-job.component';
import { CreateJobRequest } from '../../models/Job';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-job',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './create-job.component.html',
  styleUrl: './create-job.component.css'
})
export class CreateJobComponent {
  form: FormGroup;
  jobTypes = Object.values(JobType);
  experienceLevels = Object.values(ExperienceLevel);

  loading = false;
  successMessage = '';
  errorMessage = '';
get f() {
  return this.form.controls;
}
  constructor(private jobservice:JobService ,private fb:FormBuilder){
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.required],
      location: ['', Validators.required],
      minSalary: [0, [Validators.required, Validators.min(0)]],
      maxSalary: [0, [Validators.required, Validators.min(0)]],
      currency: ['USD', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
      requirements: ['', Validators.required],
      responsibilities: ['', Validators.required],
      jobType: [JobType.FullTime, Validators.required],
      experienceLevel: [ExperienceLevel.EntryLevel, Validators.required],
      isActive: [true],
      expirationDate: ['']
    });
  }
   submit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;

    if (v.maxSalary < v.minSalary) {
      this.errorMessage = 'Max salary must be greater than or equal to min salary.';
      return;
    }

    const request: CreateJobRequest = {
      ...v,
      currency: v.currency.toUpperCase(),
      expirationDate: v.expirationDate ? new Date(v.expirationDate).toISOString() : null
    };

    this.loading = true;
    this.jobservice.createJob(request).subscribe({
      next: res => {
        this.loading = false;
        this.successMessage = res.Message;
        this.form.reset({
          minSalary: 0,
          maxSalary: 0,
          currency: 'USD',
          jobType: JobType.FullTime,
          experienceLevel: ExperienceLevel.EntryLevel,
          isActive: true
        });
      },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        this.errorMessage = err.error?.message ?? 'Please check the form fields and try again.';
      }
    });
  }
  
}
