import { Component, Input } from '@angular/core';
import { JobService } from '../../Service/job.service';
import { JobMatch } from '../../models/Job';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-similar-jobs',
  standalone: true,
  imports: [CommonModule,RouterLink],
  templateUrl: './similar-jobs.component.html',
  styleUrl: './similar-jobs.component.css'
})
export class SimilarJobsComponent {
@Input({ required: true }) jobId!: number;
  @Input() take = 6;

  jobs: JobMatch[] = [];
  loading = true;

  constructor(private jobService: JobService) {}

  ngOnChanges(): void {
    this.loading = true;
    this.jobService.getSimilar(this.jobId, this.take).subscribe({
      next: j => { this.jobs = j; this.loading = false; },
      error: () => { this.jobs = []; this.loading = false; }
    });
  }
}
