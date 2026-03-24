import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { JobApplicationService } from '../../Services/JobApplication/jopapplicationservice.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-createjobapplication',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './createjobapplication.component.html',
  styleUrl: './createjobapplication.component.css'
})
export class CreatejobapplicationComponent {
applicationForm!: FormGroup;
  submitting = false;
  successMessage = '';
  errorMessage = '';
  jobId!: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private jobAppService: JobApplicationService
  ) {}

  ngOnInit(): void {
    this.jobId = Number(this.route.snapshot.paramMap.get('id'));
    this.initForm();
  }

  initForm() {
    this.applicationForm = this.fb.group({
      coverLetter: ['', Validators.required],
      experienceSummary: ['', Validators.required],
      linkedInUrl: [''],
      motivation: ['', Validators.required],
          resume: [null, Validators.required]

    });
  }
onFileSelected(event: any) {
  const file = event.target.files[0];
  if (file) {
    this.applicationForm.patchValue({
      resume: file
    });
  }
}
submitApplication() {
  if (this.applicationForm.invalid) return;

  this.submitting = true;

  const formData = new FormData();
  formData.append('coverLetter', this.applicationForm.value.coverLetter);
  formData.append('experienceSummary', this.applicationForm.value.experienceSummary);
  formData.append('linkedInUrl', this.applicationForm.value.linkedInUrl);
  formData.append('motivation', this.applicationForm.value.motivation);
  formData.append('resume', this.applicationForm.value.resume);

  this.jobAppService.applyToJob(this.jobId, formData)
    .subscribe({
      next: res => {
        this.successMessage = res.message || 'Application submitted successfully!';
        this.submitting = false;
        this.applicationForm.reset();
        this.router.navigate(['/job', this.jobId]);
      },
      error: err => {
        this.errorMessage = err.error?.message || 'Failed to submit application.';
        this.submitting = false;
      }
    });
}
}
