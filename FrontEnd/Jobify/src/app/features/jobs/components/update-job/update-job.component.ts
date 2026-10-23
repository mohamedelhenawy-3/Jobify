import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';

import { JobService } from '../../../../Services/JobService/job.service';
import { jobdetails } from '../../../../Models/Job/detailsjob';

// Enums matching .NET definitions
export enum JobType {
  FullTime = 1,
  PartTime = 2,
  Contract = 3,
  Remote = 4,
  Hybrid = 5,
  Internship = 6
}

export enum ExperienceLevel {
  EntryLevel = 1,
  MidLevel = 2,
  Senior = 3,
  Lead = 4,
  Executive = 5
}

@Component({
  selector: 'app-update-job',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './update-job.component.html',
  styleUrl: './update-job.component.css'
})
export class UpdateJobComponent implements OnInit {

  jobId!: number;
  jobForm!: FormGroup;
  isLoading: boolean = false;

  // Options for select inputs
  jobTypes = [
    { id: JobType.FullTime, name: 'Full Time' },
    { id: JobType.PartTime, name: 'Part Time' },
    { id: JobType.Contract, name: 'Contract' },
    { id: JobType.Remote, name: 'Remote' },
    { id: JobType.Hybrid, name: 'Hybrid' },
    { id: JobType.Internship, name: 'Internship' }
  ];

  experienceLevels = [
    { id: ExperienceLevel.EntryLevel, name: 'Entry Level' },
    { id: ExperienceLevel.MidLevel, name: 'Mid Level' },
    { id: ExperienceLevel.Senior, name: 'Senior' },
    { id: ExperienceLevel.Lead, name: 'Lead' },
    { id: ExperienceLevel.Executive, name: 'Executive' }
  ];

  constructor(
    private fb: FormBuilder,
    private jobService: JobService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.jobId = Number(this.route.snapshot.paramMap.get('id'));
    console.log('hellow in updae');
    this.initForm();
    this.loadJob();
  }

  initForm(): void {
    this.jobForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      requirements: ['', Validators.required],
      responsibilities: ['', Validators.required],
      location: ['', Validators.required],
      minSalary: [0, [Validators.required, Validators.min(0)]],
      maxSalary: [0, [Validators.required, Validators.min(0)]],
      jobType: [null, Validators.required],
      experienceLevel: [null, Validators.required],
      isActive: [true, Validators.required]
    });
  }

  loadJob(): void {
    this.isLoading = true;
    this.jobService.getJobDetails(this.jobId).subscribe({
      next: (res: jobdetails) => {
        this.isLoading = false;
        
        console.log('jodetails',res);
        // Map string values received from API to Enum numbers
        const mappedJobType = JobType[res.employmentType as keyof typeof JobType] ?? JobType.FullTime;
        const mappedExpLevel = ExperienceLevel[res.exprianceLevel as keyof typeof ExperienceLevel] ?? ExperienceLevel.EntryLevel;
        const mappedIsActive = res.status === 'Active';

        this.jobForm.patchValue({
          title: res.title,
          description: res.description,
          location: res.location,
          minSalary: res.minSalary,
          maxSalary: res.maxSalary,
          requirements: res.requirements,
          responsibilities: res.responsibilities,
          jobType: mappedJobType,
          experienceLevel: mappedExpLevel,
          isActive: mappedIsActive
        });
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Error fetching job details:', err);
      }
    });
  }

  submit(): void {
    if (this.jobForm.invalid) {
      this.jobForm.markAllAsTouched();
      return;
    }

    const formValue = this.jobForm.value;

    const updateJobDto = {
      title: formValue.title,
      description: formValue.description,
      requirements: formValue.requirements,
      responsibilities: formValue.responsibilities,
      location: formValue.location,
      minSalary: Number(formValue.minSalary),
      maxSalary: Number(formValue.maxSalary),
      jobType: Number(formValue.jobType),
      experienceLevel: Number(formValue.experienceLevel),
      isActive: Boolean(formValue.isActive)
    };

    this.jobService.updateJob(this.jobId, updateJobDto).subscribe({
      next: () => {
        alert('Job updated successfully ✅');
        this.router.navigate(['Jobify/employerjobs']);
      },
      error: (err) => {
        console.error('Update failed:', err);
        alert('Update failed ❌');
      }
    });
  }
}