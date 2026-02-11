import { Component, OnInit } from '@angular/core';
import { DashboarderviceService } from '../../Services/DashBoardService/dashboardervice.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-jobseekerdashboard',
  standalone: true,
  imports: [CommonModule,RouterLink],
  templateUrl: './jobseekerdashboard.component.html',
  styleUrl: './jobseekerdashboard.component.css'
})
export class JobseekerdashboardComponent  implements OnInit{
   appliedJobs: any[] = [];
  savedJobs: any[] = [];

  topStats: { title: string, value: number }[] = [];

  constructor(private dashboardService: DashboarderviceService,private router: Router) { }

  ngOnInit(): void {
    this.dashboardService.getDashboard().subscribe(data => {
      this.topStats = [
        { title: 'Total Applications', value: data.totalApplications },
        { title: 'Under Review', value: data.underReviewApplications },
        { title: 'Interviews', value: data.interviewApplications },
        { title: 'Saved Jobs', value: data.savedJobs.length }
      ];

      this.appliedJobs = data.appliedJobs;
     
;      this.savedJobs = data.savedJobs;
 console.log( "saved",this.savedJobs)
    });
  }
  goToJobDetails(jobId: number) {
  this.router.navigate(['/details', jobId]);
}

}


