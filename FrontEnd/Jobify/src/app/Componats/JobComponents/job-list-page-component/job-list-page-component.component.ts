import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { JobFilterComponent } from "../job-filter/job-filter.component";
import { JobFilter } from '../../../Models/Job/jobfilter';
import { CommonModule } from '@angular/common';
import { Job } from '../../../Models/Job/job';
import { LookupService } from '../../../Services/LookUpService/lookup.service';
import { ActivatedRoute, Router, RouterConfigurationFeature } from '@angular/router';

@Component({
  selector: 'app-job-list-page-component',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './job-list-page-component.component.html',
  styleUrl: './job-list-page-component.component.css'
})
export class JobListPageComponentComponent implements OnInit  {

  @Input() jobs: Job[] = [];
  @Input() page = 1;
  @Input() totalPages = 0;

  @Output() pageChange = new EventEmitter<number>();

  industries: any[] = [];
employmentTypes: any[] = [];
workmode: any[] = [];

jobbs: any[] = [];
constructor(private lookupsService:LookupService ,private router:Router){}
  ngOnInit(): void {
 this.loadLookups();
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
viewJob(id:number){
return this.router.navigate(['/details',id]);
}


    changePage(p: number) {
    if (p >= 1 && p <= this.totalPages) {
      this.pageChange.emit(p);
    }
  }
  getJobIcon(job: any): string {
  const title = job.title?.toLowerCase() || '';

  // 1️⃣ Specific roles
  if (title.includes('full')) return 'fas fa-laptop-code';
  if (title.includes('backend')) return 'fas fa-server';
  if (title.includes('frontend')) return 'fas fa-code';
  if (title.includes('ai') || title.includes('ml')) return 'fas fa-robot';
  if (title.includes('data')) return 'fas fa-database';
  if (title.includes('devops')) return 'fas fa-tools';

  // 2️⃣ Industry-based fallback (optional)
  switch (job.industryId) {
    case 1: return 'fas fa-laptop-code'; // Software
    case 2: return 'fas fa-chart-line';  // Business
    case 3: return 'fas fa-industry';    // Manufacturing
  }

  // 3️⃣ Final fallback (ALWAYS exists)
  return 'fas fa-briefcase';
}

}
