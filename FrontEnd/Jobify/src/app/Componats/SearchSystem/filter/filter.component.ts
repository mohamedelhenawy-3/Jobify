import { Component, EventEmitter, Input, Output } from '@angular/core';
import { GlobalSearchDto } from '../../../Models/SearchSystem/GlobalSearchDto';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-filter',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.css']
})
export class FilterComponent {
 @Input() searchDto!: GlobalSearchDto;
 @Output() filtersChange = new EventEmitter<GlobalSearchDto>();



 onChange() {
    this.filtersChange.emit(this.searchDto);
    console.log("fiter change",this.searchDto);
  }
onTypeChange(newType: 'all' | 'job' | 'user' | 'company') {
  this.searchDto.type = newType; // update type only
  this.onChange();               // emit full object with keyword and other filters
}
}
