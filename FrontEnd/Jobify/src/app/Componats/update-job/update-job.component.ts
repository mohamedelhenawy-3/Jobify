import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';

import { JobService } from '../../Services/JobService/job.service';
import { LookupService } from '../../Services/LookUpService/lookup.service';
import { jobdetails } from '../../Models/Job/dteailsjob';

@Component({
  selector: 'app-update-job',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './update-job.component.html',
  styleUrl: './update-job.component.css'
})
export class UpdateJobComponent implements OnInit {

  jobId!: number;
  jobForm!: FormGroup;

  industries: any[] = [];
  employmentTypes: any[] = [];
  workModes: any[] = [];

  constructor(
    private fb: FormBuilder,
    private jobService: JobService,
    private router: Router,
    private route: ActivatedRoute,
    private lookupService: LookupService
  ) {}

  ngOnInit(): void {
    this.jobId = Number(this.route.snapshot.paramMap.get('id'));

    this.initForm();
    this.loadLookups();
    this.loadJob();
  }

  initForm() {
   this.jobForm = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    location: ['', Validators.required],
    minSalary: [0, Validators.min(0)],
    maxSalary: [0, Validators.min(0)],
    industryId: [null, Validators.required],
    employmentTypeId: [null, Validators.required],
    workModeId: [null, Validators.required],
  });
  }
  /*
        this.jobForm.patchValue({
        title: res.title,
        description: res.description,
        location: res.location,
        minSalary: res.minSalary,
        maxSalary: res.maxSalary,
        industryId: res.industryId,
        employmentTypeId: res.employmentTypeId,
        workModeId: res.workModeId,
        statusId: res.statusId
      });
  */
  loadJob() {
    this.jobService.getJob(this.jobId).subscribe(res => {
      console.log("data for res ",res);
          this.jobForm.patchValue({
      title: res.title,
      description: res.description,
      location: res.location,
      minSalary: res.minSalary,
      maxSalary: res.maxSalary,

      // 👇 MUST BE IDs
      industryId: res.industryId,
      employmentTypeId: res.employmentTypeId,
      workModeId: res.workModeId,

    });

    });
  }

  loadLookups() {
    this.lookupService.getIndustries().subscribe(res => this.industries = res);
    this.lookupService.getEmploymentTypes().subscribe(res => this.employmentTypes = res);
    this.lookupService.getWorkModes().subscribe(res => this.workModes = res);
  }

 
submit() {
  if (this.jobForm.invalid) {
    this.jobForm.markAllAsTouched();
    return;
  }

  const updateJobDto = {
    title: this.jobForm.value.title,
    description: this.jobForm.value.description,
    location: this.jobForm.value.location,
    minSalary: this.jobForm.value.minSalary,
    maxSalary: this.jobForm.value.maxSalary,
    industryId: this.jobForm.value.industryId,
    employmentTypeId: this.jobForm.value.employmentTypeId,
    workModeId: this.jobForm.value.workModeId,
  };

  this.jobService.updateJob(this.jobId, updateJobDto).subscribe({
    next: () => {
      alert('Job updated successfully ✅');
      this.router.navigate(['/managejobs']); // adjust route if needed
    },
    error: err => {
      console.error(err);
      alert('Update failed ❌');
    }
  });
}

  // }
}
