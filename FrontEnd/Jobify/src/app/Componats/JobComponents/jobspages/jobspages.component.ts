import { Component, OnInit } from '@angular/core';
import { JobService } from '../../../Services/JobService/job.service';
import { JobFilterComponent } from "../job-filter/job-filter.component";
import { JobListPageComponentComponent } from '../job-list-page-component/job-list-page-component.component';
import { Job } from '../../../Models/Job/job';
import { Console } from 'console';

@Component({
  selector: 'app-jobspages',
  standalone: true,
  imports: [JobListPageComponentComponent,JobFilterComponent],
  templateUrl: './jobspages.component.html',
  styleUrl: './jobspages.component.css'
})
export class JobspagesComponent implements OnInit {
 jobs: Job[] = [];
  page = 1;
  pageSize = 10;
  totalPages = 0;

  filters = {
    industries: [] as number[],
    types: [] as number[],
    modes: [] as number[]
  };

  constructor(private jobService: JobService) {}

  ngOnInit(): void {
    this.loadJobs();
  }

  loadJobs() {
    this.jobService.getJobs(this.page, this.pageSize, this.filters)
      .subscribe(res => {
        this.jobs = res.items;
        this.totalPages = res.totalPages;
        console.log("Jobs loaded:", res.items);
      });
  }

  onPageChange(page: number) {
    this.page = page;
    this.loadJobs();
  }

  onFiltersApplied(filters: any) {
    console.log('Parent received filters', filters);
    this.filters = filters;
    this.page = 1; // reset to first page
    this.loadJobs(); // reload jobs based on new filters
  }
}
