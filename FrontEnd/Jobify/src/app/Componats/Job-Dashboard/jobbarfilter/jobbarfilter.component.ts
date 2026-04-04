import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-jobbarfilter',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './jobbarfilter.component.html',
  styleUrl: './jobbarfilter.component.css'
})
export class JobbarfilterComponent {
  @Input() industries: any[] = [];
  @Input() workModes: any[] = [];
  @Input() EmploymentTypeIds:any[]=[];
  @Input() currentKeyword: string = '';

  @Output() filterChanged = new EventEmitter<any>();

  selectedDays: number | null = null;
  selectedIndId: number | null = null;
  selectedModeId: number | null = null;
  selectedEmpTypeId:number | null =null;


  constructor(){
          console.log("currentkeyword :-",this.currentKeyword);

  }

  emitFilters() {
    const filterDto = { 
      
      industryIds: this.selectedIndId ? [this.selectedIndId] : [],
      workModeIds: this.selectedModeId ? [this.selectedModeId] : [],
      employmentTypeIds: this.selectedEmpTypeId ? [this.selectedEmpTypeId] : [],
      postedWithinDays: this.selectedDays
    };
    console.log("emit filters,",filterDto);
    this.filterChanged.emit(filterDto);
  }

  resetFilters() {
    this.selectedDays = null;
    this.selectedIndId = null;
    this.selectedModeId = null;
    this.selectedEmpTypeId = null;
    this.emitFilters();
  }
}
