import { Component, EventEmitter, Input, Output } from '@angular/core';
import { GlobalSearchDto } from '../../../../Models/SearchSystem/GlobalSearchDto';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-filters',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './filters.component.html',
  styleUrls: ['./filters.component.css']
})
export class FiltersComponent {
 @Input() searchDto!: GlobalSearchDto;
 @Output() filtersChange = new EventEmitter<GlobalSearchDto>();



 onChange() {
    this.filtersChange.emit(this.searchDto);
  }
   onTypeChange() {
    this.searchDto = {
      keyword: this.searchDto.keyword,
      type: this.searchDto.type
    };

    this.onChange();
  }
}
