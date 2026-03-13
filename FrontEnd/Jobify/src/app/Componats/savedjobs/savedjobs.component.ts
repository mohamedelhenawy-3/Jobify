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
            this.loading = false;

  }, error: () => this.loading = false
})  }

unsavedJob(id:number){
  this.savedjobsservice.unsaveJob(id).subscribe(()=>{
    next:{
      alert("Un Saved Succefylly");
 

    }
  })
}

}
