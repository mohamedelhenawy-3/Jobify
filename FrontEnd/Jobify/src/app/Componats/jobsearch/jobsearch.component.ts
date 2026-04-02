import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JobService } from '../../Services/JobService/job.service';
import { jobdetails } from '../../Models/Job/dteailsjob';
import { SearchJob } from '../../Models/Job/search';
import { CommonModule } from '@angular/common';
import { LookupService } from '../../Services/LookUpService/lookup.service';

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
industries: any[] = [];
employmentTypes: any[] = [];
workmode: any[] = [];




filter = {
  keyword: '',
  industryIds: [] as number[],
  employmentTypeIds: [] as number[],
  workModeIds: [] as number[],
  postedWithinDays: null as number | null
};

constructor(
  private route: ActivatedRoute,
  private jobService: JobService,private lookupsService:LookupService
) {}
ngOnInit(): void {
  this.route.queryParams.subscribe(params => {
    this.filter.keyword = params['keyword'] || '';
    this.loadJobs();
    this.loadLookups();
  });
}
  loadLookups() {
  this.lookupsService.getIndustries().subscribe(res => {
    console.log("ind",res);
    this.industries = res;
  });

  this.lookupsService.getEmploymentTypes().subscribe(res => {
    this.employmentTypes = res;
  });
   this.lookupsService.getWorkModes().subscribe(res => {
    this.workmode = res;
        console.log("eorkmode",res);

  });
}
getIndustryName(industryId: number): string {
  return this.industries.find(i => i.id === industryId)?.industryName  || 'Unknown';
}

getEmploymentTypeName(typeId: number): string {
  return this.employmentTypes.find(t => t.id === typeId)?.typeName || 'Unknown';
}

getWorkModeName(modeId: number): string {
  return this.workmode.find(w => w.id === modeId)?.name || 'Unknown';
}
getEmploymentTypeClass(typeId: number): string {
  switch (typeId) {
    case 1: return 'badge-fulltime';   // Full Time
    case 2: return 'badge-parttime';   // Part Time
    case 3: return 'badge-contract';   // Contract
    default: return 'badge-default';
  }
}
getWorkModeClass(modeId: number): string {
  switch (modeId) {
    case 1: return 'badge-onsite';
    case 2: return 'badge-remote';
    case 3: return 'badge-hybrid';
    default: return 'badge-default';
  }
}

loadJobs() {
  this.loading = true;

  this.jobService.search(this.filter).subscribe({
    next: (res) => {
      this.jobs = res;
      console.log("filtered jobs", res);
      this.loading = false;
    },
    error: () => {
      this.loading = false;
    }
  });
}

toggleSelection(list: number[], value: number) {
  const index = list.indexOf(value);

  if (index > -1) {
    list.splice(index, 1);
  } else {
    list.push(value);
  }

  this.loadJobs();
}

setDays(days: number) {
  this.filter.postedWithinDays =
    this.filter.postedWithinDays === days ? null : days;

  this.loadJobs();
}

clearFilters() {
  this.filter = {
    keyword: this.filter.keyword,
    industryIds: [],
    employmentTypeIds: [],
    workModeIds: [],
    postedWithinDays: null
  };

  this.loadJobs();
}
}
