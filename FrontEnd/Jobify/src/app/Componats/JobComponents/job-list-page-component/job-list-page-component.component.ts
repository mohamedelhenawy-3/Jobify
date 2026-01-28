import { Component, EventEmitter, Input, Output } from '@angular/core';
import { JobFilterComponent } from "../job-filter/job-filter.component";
import { JobFilter } from '../../../Models/Job/jobfilter';
import { CommonModule } from '@angular/common';
import { Job } from '../../../Models/Job/job';

@Component({
  selector: 'app-job-list-page-component',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './job-list-page-component.component.html',
  styleUrl: './job-list-page-component.component.css'
})
export class JobListPageComponentComponent {
  @Input() jobs: Job[] = [];
  @Input() page = 1;
  @Input() totalPages = 0;

  @Output() pageChange = new EventEmitter<number>();


    changePage(p: number) {
    if (p >= 1 && p <= this.totalPages) {
      this.pageChange.emit(p);
    }
  }
}
