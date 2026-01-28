import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { JobFilter } from '../../../Models/Job/jobfilter';
import { CommonModule } from '@angular/common';
import { Industry } from '../../../Models/Industry/industry';
import { EmploymentType } from '../../../Models/EmploymentType/EmploymentType ';
import { WorkMode } from '../../../Models/WorkMode/WorkMode';
import { LookupService } from '../../../Services/LookUpService/lookup.service';

@Component({
  selector: 'app-job-filter',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './job-filter.component.html',
  styleUrl: './job-filter.component.css'
})
export class JobFilterComponent implements OnInit {

 
   @Output() filterChanged = new EventEmitter<any>();

  industries: Industry[] = [];
  employmentTypes: EmploymentType[] = [];
  workModes: WorkMode[] = [];

  filters = {
    industries: [] as number[],
    types: [] as number[],
    modes: [] as number[]
  };

  constructor(private lookupService: LookupService) {}

  ngOnInit(): void {
    this.lookupService.getIndustries().subscribe(data => this.industries = data);
    this.lookupService.getEmploymentTypes().subscribe(data => this.employmentTypes = data);
    this.lookupService.getWorkModes().subscribe(data => this.workModes = data);
  }

  toggleIndustry(id: number) {
    const idx = this.filters.industries.indexOf(id);
    idx > -1 ? this.filters.industries.splice(idx, 1) : this.filters.industries.push(id);
  }

  toggleType(id: number) {
    const idx = this.filters.types.indexOf(id);
    idx > -1 ? this.filters.types.splice(idx, 1) : this.filters.types.push(id);
  }

  toggleMode(id: number) {
    const idx = this.filters.modes.indexOf(id);
    idx > -1 ? this.filters.modes.splice(idx, 1) : this.filters.modes.push(id);
  }

  applyFilters() {
    console.log('Emitting filters', this.filters);
    this.filterChanged.emit(this.filters);
  }

}
