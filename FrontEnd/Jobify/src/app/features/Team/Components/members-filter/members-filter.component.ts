import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { TeamFilter } from '../../Models/team';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-members-filter',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './members-filter.component.html',
  styleUrl: './members-filter.component.css'
})
export class MembersFilterComponent  implements OnInit  {
  
   @Output() filterChange = new EventEmitter<Partial<TeamFilter>>();
   search = '';
   role: number | null = null;
   status: boolean | null = null;
   sortBy?: 'name' | 'joined' | 'jobs';

  private search$ = new Subject<string>();

  ngOnInit(): void {
      this.search$
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(text => this.filterChange.emit({ search: text }));
   }
  onSearch(text: string): void {
    this.search$.next(text);
  }

  onDropdownChange(): void {
    this.filterChange.emit({ role: this.role, isActive: this.status,sortBy:this.sortBy });
  }
    reset(): void {
    this.search = '';
    this.role = null;
    this.status = null;
    this.sortBy = 'name';
    this.filterChange.emit({ search: '', role: null, isActive: null, sortBy: 'name' });
  }
}
