import { Component, OnInit } from '@angular/core';
import { DEFAULT_EMPLOYER_JOBS_FILTER, EmployerJobsFilter } from '../../../models/job-search-filter.model';
import { EmployerJobCard, PagedResult } from '../../../models/Job';
import { JobService } from '../../../Service/job.service';
import { JobCardComponent } from '../../job-card/job-card.component';
import { JobsFilterComponent } from '../../jobs-filter/jobs-filter.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-employer-jobs',
  standalone: true,
  imports: [JobCardComponent, JobsFilterComponent, CommonModule, JobCardComponent],
  templateUrl: './employer-jobs.component.html',
  styleUrl: './employer-jobs.component.css'
})
export class EmployerJobsComponent implements OnInit{
  filter: EmployerJobsFilter = { ...DEFAULT_EMPLOYER_JOBS_FILTER };
  result: PagedResult<EmployerJobCard> | null = null;
  loading = false;
  error: string | null = null;



    constructor(private jobsService: JobService) {}

    ngOnInit(): void { this.load(); }

  onFilterChange(newFilter: EmployerJobsFilter): void {
    this.filter = newFilter;
    this.load();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.filter = { ...this.filter, pageNumber: page };
    this.load();
  }

  get totalPages(): number {
    return this.result ? Math.max(1, Math.ceil(this.result.totalCount / this.result.pageSize)) : 1;
  }

  private load(): void {
    this.loading = true;
    this.error = null;
    this.jobsService.getEmployerJobs(this.filter).subscribe({
      next: (r) => { this.result = r; this.loading = false; },
      error: (err) => {
        this.error = err.status === 403 ? 'You do not have permission to view jobs.' : 'Could not load jobs.';
        this.loading = false;
      }
    });
  }
}
