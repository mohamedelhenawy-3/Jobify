import { Component, inject, OnInit } from '@angular/core';
import { SearchService } from '../../Services/search.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { GlobalSearchResponse, SearchType } from '../../Models/Search';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JobalertService } from '../../../jobs/Service/jobalert.service';
// import { JobAlertService } from '../../../services/job-alert.service';

@Component({
  selector: 'app-search-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './search-page.component.html',
  styleUrl: './search-page.component.css'
})
export class SearchPageComponent implements OnInit {
   private readonly searchService = inject(SearchService);
  private readonly route = inject(ActivatedRoute);
 
  keyword = '';

  alertMsg = '';
  alertError = '';
  alertSaving = false;

  activeType: SearchType = 'all';
 
  results: GlobalSearchResponse | null = null;
  loading = false;
  error: string | null = null;
 

  constructor(private jpbalert:JobalertService){

  }
  tabs: { label: string; value: SearchType }[] = [
    { label: 'All', value: 'all' },
    { label: 'Jobs', value: 'job' },
    { label: 'People', value: 'user' },
    { label: 'Companies', value: 'company' }
  ];
 
  employmentType = '';
  experienceLevel = '';
  postedWithinDays: number | null = null;
  industry = '';
  companySize = '';
 
  employmentTypeOptions = ['FullTime', 'PartTime', 'Contract', 'Internship'];
  experienceLevelOptions = ['Entry', 'Mid', 'Senior'];
  postedWithinDaysOptions = [
    { label: 'Any time', value: null as number | null },
    { label: 'Past 24 hours', value: 1 },
    { label: 'Past week', value: 7 },
    { label: 'Past month', value: 30 }
  ];
  companySizeOptions = ['1-10', '11-50', '51-200', '201-500', '500+'];
  industryOptions = ['Technology', 'Finance', 'Healthcare', 'Education', 'Retail'];
 
  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.keyword = params['keyword'] ?? '';
      console.log("keywork",this.keyword);
      this.activeType = (params['type'] as SearchType) ?? 'all';
      this.alertMsg = '';
      this.alertError = '';
      this.runSearch();
    });
  }
 
    saveAlert() {
    if (!this.keyword.trim()) return;
    this.alertSaving = true;
    this.alertMsg = '';
    this.alertError = '';

    this.jpbalert.create(this.keyword).subscribe({
      next: r => { this.alertSaving = false; this.alertMsg = r.message; },
      error: e => {
        this.alertSaving = false;
        this.alertError = e.status === 401
          ? 'Log in to create job alerts.'
          : typeof e.error === 'string' ? e.error
          : e.error?.message ?? 'Could not save the alert.';
      }
    });
  }
  setType(type: SearchType): void {
    this.activeType = type;
    this.resetFilters(); // filters from the old tab don't make sense on the new one
    this.runSearch();
  }
 
  onFilterChange(): void {
    this.runSearch();
  }
 
  clearFilters(): void {
    this.resetFilters();
    this.runSearch();
  }
 
  private resetFilters(): void {
    this.employmentType = '';
    this.experienceLevel = '';
    this.postedWithinDays = null;
    this.industry = '';
    this.companySize = '';
  }
 
  get noResults(): boolean {
    return (
      !!this.results &&
      this.results.jobs.length === 0 &&
      this.results.users.length === 0 &&
      this.results.companies.length === 0
    );
  }
 
  private runSearch(): void {
    this.loading = true;
    this.error = null;
 
    this.searchService
      .globalSearch({
        keyword: this.keyword,
        type: this.activeType,
        employmentType: this.employmentType || undefined,
        experienceLevel: this.experienceLevel || undefined,
        postedWithinDays: this.postedWithinDays ?? undefined,
        industry: this.industry || undefined,
        companySize: this.companySize || undefined
      })
      .subscribe({
        next: (res) => {
          this.results = res;
          this.loading = false;
        },
        error: () => {
          this.error = 'Something went wrong while searching. Please try again.';
          this.loading = false;
        }
      });
  }
}
