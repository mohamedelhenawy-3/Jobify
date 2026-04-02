import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, output } from '@angular/core';
import { LookupService } from '../../../Services/LookUpService/lookup.service';

@Component({
  selector: 'app-jobcard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './jobcard.component.html',
  styleUrl: './jobcard.component.css'
})
export class JobcardComponent {

  industries: any[] = [];
employmentTypes: any[] = [];
workmode: any[] = [];


  @Input() job: any; 
  
  @Input() isActive: boolean = false;

  @Output() clicked = new EventEmitter<void>();
  constructor(private lookupsService:LookupService ){}
    ngOnInit(): void {
   this.loadLookups();
   }
  onCardClick() {
    this.clicked.emit();
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


}
