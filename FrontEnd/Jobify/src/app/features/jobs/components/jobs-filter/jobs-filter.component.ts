import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DEFAULT_EMPLOYER_JOBS_FILTER, EmployerJobsFilter } from '../../models/job-search-filter.model';
import { EMPLOYMENT_TYPE_LABELS, EmploymentType, EXPERIANCE_LEVEL_LABELS, ExperianceLevel, JOB_STATUSES } from '../../models/job-enums';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-jobs-filter',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './jobs-filter.component.html',
  styleUrl: './jobs-filter.component.css'
})
export class JobsFilterComponent {
  @Input() set filter(value: EmployerJobsFilter) { this.form = { ...value }; }
 @Output() filterChange = new EventEmitter<EmployerJobsFilter>();

form: EmployerJobsFilter = { ...DEFAULT_EMPLOYER_JOBS_FILTER };
 @Input() employerMode = true;  
statuses = JOB_STATUSES;
jobTypes = Object.values(EmploymentType);
levels = Object.values(ExperianceLevel);
employmentLabels = EMPLOYMENT_TYPE_LABELS;
levelLabels = EXPERIANCE_LEVEL_LABELS;

  apply(): void {
    this.filterChange.emit({ ...this.form, keyword: this.form.keyword?.trim() || null, pageNumber: 1 });
  }

  reset(): void {
    this.form = { ...DEFAULT_EMPLOYER_JOBS_FILTER, pageSize: this.form.pageSize };
    this.apply();
  }
}
