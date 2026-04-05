import { Component, OnInit } from '@angular/core';
import { GlobalSearchDto } from '../../../Models/SearchSystem/GlobalSearchDto';
import { ActivatedRoute } from '@angular/router';
import { JobsearchComponent } from '../../jobsearch/jobsearch.component';
import { SearchsystemService } from '../../../Services/SearchSystem/searchsystem.service';
import { LookupService } from '../../../Services/LookUpService/lookup.service';
import { GlobalSearchResponse } from '../../../Models/SearchSystem/GlobalSearchResponseDto';
import { FormsModule } from '@angular/forms';
import { FilterComponent } from "../filter/filter.component";
import { JobbarfilterComponent } from '../../Job-Dashboard/jobbarfilter/jobbarfilter.component';
import { CommonModule } from '@angular/common';
import { JobcardComponent } from "../../Job-Dashboard/jobcard/jobcard.component";
import { JobdetailsComponent } from "../../jobdetails/jobdetails.component";
import { SearchedusersComponent } from "../SearchedUsers/searchedusers/searchedusers.component";
import { CompanySearchedComponent } from "../company-searched/company-searched.component";

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [FormsModule, CommonModule, FilterComponent, JobbarfilterComponent, JobcardComponent, JobdetailsComponent, SearchedusersComponent, SearchedusersComponent, CompanySearchedComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
viewCompany(arg0: any) {
throw new Error('Method not implemented.');
}
viewUserProfile(arg0: any) {
throw new Error('Method not implemented.');
}
searchDto: GlobalSearchDto = { type: 'all', keyword: '' };

  industries: any[] = [];
  workModes: any[] = [];
  EmploymentTypeIds:any[]=[];
  

  jobs: any[] = [];
  users: any[] = [];
  companies: any[] = [];


  selectedJob: any;
  selectedUser: any;
  selectedCompany: any;
    constructor(
      private route: ActivatedRoute,
      private searchservice: SearchsystemService,
      private lookupService: LookupService
    ) {}
  ngOnInit(): void {
  this.loadLookups();
   this.route.queryParams.subscribe(params => {
    this.searchDto.keyword = params['keyword'] || '';
    this.searchDto.type = params['type'] || 'all';

    console.log("FROM NAVBAR:", this.searchDto); // 🔥 check here

    this.performSearch();
  });
   
  
  }


  loadLookups() {
    this.lookupService.getIndustries().subscribe(res => this.industries = res);
    this.lookupService.getWorkModes().subscribe(res => this.workModes = res);
    this.lookupService.getEmploymentTypes().subscribe(res=>this.EmploymentTypeIds=res);
  }

    onGlobalFilterChange(dto: GlobalSearchDto) {
    this.searchDto = { ...this.searchDto, ...dto };
    this.performSearch();
  }
    performSearch() {
  console.log("🚀 SEARCH DTO SENT:", this.searchDto);
      
    this.searchservice.search(this.searchDto).subscribe({
      next: (res: GlobalSearchResponse) => {
        this.jobs = res.jobs;
        console.log("jobs",res.jobs);
        this.users = res.users;
          console.log("users",res.users);
        this.companies = res.companies;
        console.log("comapnies",res.companies);

          if (this.searchDto.type === 'job' && this.jobs.length > 0) {
        this.selectedJob = this.jobs[0];
      }
      },
      error: (err) => console.error('Search failed', err)
    });
  }

    getCurrentList() {
    switch (this.searchDto.type) {
      case 'job': return this.jobs;
      case 'user': return this.users;
      case 'company': return this.companies;
      default: return [];
    }
  }

onJobFilterChange(filters: any) {
  this.searchDto = {
    ...this.searchDto,
    workModeIds: filters.workModeIds,
    employmentTypeIds: filters.employmentTypeIds,
    industryIds: filters.industryIds,
    postedWithinDays: filters.postedWithinDays
  };

  this.performSearch();
}
  getIndustryName(industryId: number): string {
  return this.industries.find(i => i.id === industryId)?.industryName  || 'Unknown';
}

getEmploymentTypeName(typeId: number): string {
  return this.EmploymentTypeIds.find(t => t.id === typeId)?.typeName || 'Unknown';
}

getWorkModeName(modeId: number): string {
  return this.workModes.find(w => w.id === modeId)?.name || 'Unknown';
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

}
