import { Component, DestroyRef, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

import {
  DEFAULT_EMPLOYER_JOBS_FILTER,
  EmployerJobsFilter,
  POSTED_WITHIN_OPTIONS
} from '../../models/job-search-filter.model';
import {
  EMPLOYMENT_TYPE_LABELS,
  EmploymentType,
  EXPERIANCE_LEVEL_LABELS,
  ExperianceLevel,
  JOB_STATUSES
} from '../../models/job-enums';
import { IndustryService } from '../../../Imdustry/Services/industry.service';  // adjust path
import { Industry } from '../../../Imdustry/Model/industry.model';

@Component({
  selector: 'app-jobs-filter',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './jobs-filter.component.html',
  styleUrl: './jobs-filter.component.css'
})
export class JobsFilterComponent implements OnInit {
  private industryService = inject(IndustryService);
  private destroyRef = inject(DestroyRef);
  private keyword$ = new Subject<string>();

  @Input() set filter(value: EmployerJobsFilter) { this.form = { ...value }; }
  @Output() filterChange = new EventEmitter<EmployerJobsFilter>();
  @Input() employerMode = true;

  form: EmployerJobsFilter = { ...DEFAULT_EMPLOYER_JOBS_FILTER };

  statuses = JOB_STATUSES;
  jobTypes = Object.values(EmploymentType);
  levels = Object.values(ExperianceLevel);
  employmentLabels = EMPLOYMENT_TYPE_LABELS;
  levelLabels = EXPERIANCE_LEVEL_LABELS;

  industries: Industry[] = [];
  postedOptions = POSTED_WITHIN_OPTIONS;

  // true when anything differs from the defaults (sort order doesn't count)
  get hasActiveFilters(): boolean {
    const f = this.form;
    return !!(f.keyword?.trim() || f.status || f.employmentType || f.experienceLevel
           || f.industry || f.postedWithin);
  }

  ngOnInit(): void {
    this.industryService.getAll(false).subscribe({
      next: (list) => (this.industries = list),
      error: () => (this.industries = [])
    });

    // typing: wait until the user pauses, then apply
    this.keyword$
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.apply());
  }

  onKeywordChange(value: string): void {
    this.keyword$.next(value?.trim() ?? '');
  }

  apply(): void {
    this.filterChange.emit({
      ...this.form,
      keyword: this.form.keyword?.trim() || null,
      industry: this.form.industry || null,
      postedWithin: this.form.postedWithin || null,
      pageNumber: 1
    });
  }

  reset(): void {
    this.form = { ...DEFAULT_EMPLOYER_JOBS_FILTER, pageSize: this.form.pageSize };
    this.apply();
  }
}