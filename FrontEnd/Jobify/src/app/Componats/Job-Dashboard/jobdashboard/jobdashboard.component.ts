import { Component, OnInit } from '@angular/core';
import { SearchJob } from '../../../Models/Job/search';
import { ActivatedRoute } from '@angular/router';
import { JobService } from '../../../Services/JobService/job.service';
import { LookupService } from '../../../Services/LookUpService/lookup.service';
import { CommonModule } from '@angular/common';
import { JobdetailsComponent } from "../../jobdetails/jobdetails.component";
import { JobFilterComponent } from "../../JobComponents/job-filter/job-filter.component";
import { JobbarfilterComponent } from "../jobbarfilter/jobbarfilter.component";
import { JobcardComponent } from "../jobcard/jobcard.component";
import { FormsModule } from '@angular/forms';
import { FiltersComponent } from "../Filters/filters/filters.component";
import { GlobalSearchDto } from '../../../Models/SearchSystem/GlobalSearchDto';

@Component({
  selector: 'app-jobdashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, JobdetailsComponent, JobFilterComponent, JobbarfilterComponent, JobcardComponent, FiltersComponent],
  templateUrl: './jobdashboard.component.html',
  styleUrls: ['./jobdashboard.component.css']
})
export class JobdashboardComponent implements OnInit {

    searchDto: GlobalSearchDto = {
    keyword: '',
    type: 'all',
    workModeIds: [],
    employmentTypeIds: [],
    industryIds: [],
    location: '',
    companySize: ''
  };

 jobs: SearchJob[] = [];
  selectedJob: SearchJob | null = null; // For the Description Component
  
  industries: any[] = [];
  workModes: any[] = [];
  EmploymentTypeIds:any[]=[];
  
  keyword: string = '';
  loading: boolean = false;
  isAlertEnabled: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private jobService: JobService,
    private lookupService: LookupService
  ) {}
  ngOnInit(): void {
this.loadLookups(); 
this.route.queryParams.subscribe(params=>{
  this.keyword=params['keyword']||'';
  console.log("keyword",this.keyword)
  this.refreshJobs({ keyword: this.keyword });
}) }

loadLookups() {
    this.lookupService.getIndustries().subscribe(res => this.industries = res);
    this.lookupService.getWorkModes().subscribe(res => this.workModes = res);
    this.lookupService.getEmploymentTypes().subscribe(res=>this.EmploymentTypeIds=res);
  }


  onFilterChanged(filterDto: any) {
    this.refreshJobs(filterDto);
  }
  refreshJobs(params: any) {
    this.loading = true;
    this.jobService.search(params).subscribe({
      next: (res) => {
        this.jobs = res;
        console.log("jobs",this.jobs);
        this.loading = false;
        if (this.jobs.length > 0) this.selectedJob = this.jobs[0];
      },
      error: () => this.loading = false
    });
  }

  onSelectJob(job: SearchJob) {
    this.selectedJob = job;
    console.log("Selected Job",job);
  }
  
onToggleAlert() {
  // Only call the API if the toggle is turned ON
  if (this.isAlertEnabled && this.keyword) {
    
    this.jobService.saveJobAlert(this.keyword).subscribe({
      next: (res) => {
alert("job alert succefully");      },
      error: () => {
        this.isAlertEnabled = false; // Flip back if API fails
      }
    });
    
  }
}



 
}
