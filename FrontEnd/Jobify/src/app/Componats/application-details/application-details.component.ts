import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JobApplicationService } from '../../Services/JobApplication/jopapplicationservice.service';
import { AppliedJobSeekerDetails } from '../../Models/JobApplications/appliedSeekerDetails';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-application-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './application-details.component.html',
  styleUrl: './application-details.component.css'
})
export class ApplicationDetailsComponent {
applicationDetails: AppliedJobSeekerDetails | null = null;
  applicationId!: number;

  constructor(
    private route: ActivatedRoute,
    private jobService: JobApplicationService
  ) {}

  ngOnInit(): void {
    this.applicationId = +this.route.snapshot.paramMap.get('id')!;
    this.loadDetails();
    console.log(this.loadDetails);
  }

  loadDetails(): void {
    this.jobService.getApplicationDetails(this.applicationId).subscribe({
      next: (res) =>{
this.applicationDetails = res, 
console.log("application details",res);
      } 
      // error: (err) => console.error(err)
    });
  }
  goBack(){
    
  }
}
