import { Component } from '@angular/core';
import { JobService } from '../../Service/job.service';
import { DEFAULT_EMPLOYER_JOBS_FILTER, EmployerJobsFilter } from '../../models/job-search-filter.model';
import { JobCard, PagedResult } from '../../models/Job';
import { JobCardComponent } from '../job-card/job-card.component';
import { JobsFilterComponent } from '../jobs-filter/jobs-filter.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-jobs-list',
  standalone: true,
  imports: [JobCardComponent, CommonModule, JobsFilterComponent, JobCardComponent],
  templateUrl: './jobs-list.component.html',
  styleUrl: './jobs-list.component.css'
})
export class JobsListComponent {
filter: EmployerJobsFilter = { ...DEFAULT_EMPLOYER_JOBS_FILTER };
  result: PagedResult<JobCard> | null = null;
  loading = false;
  error: string | null = null;

  constructor(private jobsService: JobService) {}

  ngOnInit() { this.load(); }

  onFilterChange(f: EmployerJobsFilter) { this.filter = f; this.load(); }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.filter = { ...this.filter, pageNumber: page };
    this.load();
  }

  get totalPages() {
    return this.result ? Math.max(1, Math.ceil(this.result.totalCount / this.result.pageSize)) : 1;
  }

  private load() {
    this.loading = true;
    this.error = null;
    this.jobsService.getJobs(this.filter).subscribe({
      next: r => { this.result = r; this.loading = false; },
      error: () => { this.error = 'Could not load jobs.'; this.loading = false; }
    });
  }
}
