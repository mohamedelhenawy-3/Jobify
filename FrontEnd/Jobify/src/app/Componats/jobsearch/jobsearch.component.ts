import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JobService } from '../../Services/JobService/job.service';
import { jobdetails } from '../../Models/Job/dteailsjob';
import { SearchJob } from '../../Models/Job/search';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-jobsearch',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './jobsearch.component.html',
  styleUrl: './jobsearch.component.css'
})
export class JobsearchComponent implements OnInit{
jobs: SearchJob[] = [];
keyword: string = '';
loading = true;

constructor(
  private route: ActivatedRoute,
  private jobService: JobService
) {}
  ngOnInit(): void {
  this.route.queryParams.subscribe(params => {
    this.keyword = params['keyword'] || '';
    this.loadJobs();
  });  }
loadJobs() {
  this.loading = true;

  this.jobService.search(this.keyword).subscribe({
    next: (res) => {
      this.jobs = res;
      console.log("searched jobs",res);
      this.loading = false;
    },
    error: () => {
      this.loading = false;
    }
  });
}
}
