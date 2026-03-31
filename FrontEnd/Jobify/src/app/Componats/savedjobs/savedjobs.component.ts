import { Component, OnInit } from '@angular/core';
import { SavedJob } from '../../Models/savedjobs/SavedJob';
import { CommonModule } from '@angular/common';
import { SavedjobsService } from '../../Services/Savedjobsservice/savedjobs.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-savedjobs',
  standalone: true,
  imports: [CommonModule ,RouterLink],
  templateUrl: './savedjobs.component.html',
  styleUrl: './savedjobs.component.css'
})
export class SavedjobsComponent implements OnInit {
 loading = true;

 savedJobs:SavedJob[];
 constructor(private savedjobsservice:SavedjobsService){
  this.savedJobs =[];
 }
  ngOnInit(): void {
    this.LoadSavedJobs();
  }
  LoadSavedJobs() {
this.savedjobsservice.getSavedJobs().subscribe({
  next:(res)=>{

    this.savedJobs = res as SavedJob[];
    console.log("savedjobs",res);
            this.loading = false;

  }, error: () => this.loading = false
})  }
getEmploymentType(id: number): string {
  switch (id) {
    case 1: return 'Full-time';
    case 2: return 'Part-time';
    case 3: return 'Contract';
    default: return 'Unknown';
  }
}

getWorkMode(id: number): string {
  switch (id) {
    case 1: return 'Onsite';
    case 2: return 'Remote';
    case 3: return 'Hybrid';
    default: return 'Unknown';
  }
}

getStatusText(id: number): string {
  switch (id) {
    case 1: return 'Applied';
    case 2: return 'Under Review';
    case 3: return 'Interviewed';
    case 4: return 'Hired';
    case 5: return 'Rejected';
    default: return 'Unknown';
  }
}

getStatusClass(id: number): string {
  switch (id) {
    case 1: return 'applied';
    case 4: return 'hired';
    case 5: return 'rejected';
    default: return '';
  }
}
unsavedJob(id:number){
  this.savedjobsservice.unsaveJob(id).subscribe(()=>{
    next:{
      alert("Un Saved Succefylly");
 

    }
  })
}

}
