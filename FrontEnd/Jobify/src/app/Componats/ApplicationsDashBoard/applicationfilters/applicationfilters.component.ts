import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-applicationfilters',
  standalone: true,
  imports: [FormsModule,ReactiveFormsModule,CommonModule],
  templateUrl: './applicationfilters.component.html',
  styleUrl: './applicationfilters.component.css'
})
export class ApplicationfiltersComponent implements OnInit {

statuses = ['all', 'applied', 'shortlisted', 'interviewScheduled', 'interviewed','rejected'];
@Output() onStatusChnage = new EventEmitter<{ status: string; search: string }>();
activeStatus: string = 'all';
searchControl=new FormControl('');


ngOnInit(): void {

 this.searchControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(()=>{
  this.emitFilters();
 })
 console.log("search:", this.searchControl.value);
 console.log("status",this.activeStatus);
 this.emitFilters();

}


selectStatus(status:string){
    this.activeStatus = status;
    this.emitFilters();
}
private emitFilters() {
  const data = {
    search: this.searchControl.value?.trim() || '',
    status: this.activeStatus
  };
  console.log('Child is emitting:', data); // <--- Add this!
  this.onStatusChnage.emit(data);
}


}
