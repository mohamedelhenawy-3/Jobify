import { Component, OnInit } from '@angular/core';
import { JobDetails } from '../../models/Job';
import { ActivatedRoute, Router } from '@angular/router';
import { JobService } from '../../Service/job.service';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { SavedJobService } from '../../Service/savedjob.service';
import { SimilarJobsComponent } from '../similar-jobs/similar-jobs.component';

@Component({
  selector: 'app-job-details-page',
  standalone: true,
  imports: [CommonModule, SimilarJobsComponent],
  templateUrl: './job-details-page.component.html',
  styleUrl: './job-details-page.component.css'
})
export class JobDetailsPageComponent implements OnInit {

  job: JobDetails | null = null;
  loading = true;
  errorMessage = '';

  isSaved = false;
  savingBookmark = false;
  saveMsg = '';

  constructor(
    private savedJobs: SavedJobService,
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      if (!id) {
        this.errorMessage = 'Invalid job id.';
        this.loading = false;
        return;
      }
      this.loadJob(id);
    });
  }

  loadJob(id: number) {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    this.loading = true;
    this.errorMessage = '';
    this.isSaved = false;

    this.jobService.getJobById(id).subscribe({
      next: job => {
        this.job = job;
        this.loadSavedStatus(job.id);
        this.loading = false;
      },
      error: (err: HttpErrorResponse) => {
        this.job = null;
        this.loading = false;
        this.errorMessage = err.status === 404
          ? 'This job was not found.'
          : 'Could not load the job. Please try again.';
      }
    });
  }

  private loadSavedStatus(jobId: number) {
    this.savedJobs.isSaved(jobId).subscribe({
      next: r => this.isSaved = r.isSaved,
      error: () => {}
    });
  }

  goBack(): void {
    this.router.navigate(['/Jobify/jobs']);
  }

  apply(jobId: number) {
    this.router.navigate(['/Jobify/apply', jobId]);
  }

  toggleSave() {
    if (!this.job || this.savingBookmark) return;
    const next = !this.isSaved;
    this.isSaved = next;
    this.savingBookmark = true;

    const call$ = next ? this.savedJobs.save(this.job.id) : this.savedJobs.unsave(this.job.id);
    call$.subscribe({
      next: () => {
        this.savingBookmark = false;
        this.showSaveMsg(next ? 'Job saved' : 'Removed from saved jobs');
      },
      error: e => {
        this.isSaved = !next;
        this.savingBookmark = false;
        this.showSaveMsg(e.status === 401 ? 'Log in to save jobs' : 'Could not update, try again');
      }
    });
  }

  private showSaveMsg(msg: string) {
    this.saveMsg = msg;
    setTimeout(() => this.saveMsg = '', 2200);
  }
}