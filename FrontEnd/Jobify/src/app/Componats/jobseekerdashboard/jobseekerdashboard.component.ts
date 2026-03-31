import { Component, OnInit } from '@angular/core';
import { DashboarderviceService } from '../../Services/DashBoardService/dashboardervice.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { JobRecommendation } from '../../Models/Job/recommendedjob';
import { JobService } from '../../Services/JobService/job.service';

@Component({
  selector: 'app-jobseekerdashboard',
  standalone: true,
  imports: [CommonModule,RouterLink],
  templateUrl: './jobseekerdashboard.component.html',
  styleUrl: './jobseekerdashboard.component.css'
})
export class JobseekerdashboardComponent  implements OnInit{
  recommendedJobs: JobRecommendation[] = [];
   appliedJobs: any[] = [];
  savedJobs: any[] = [];

  topStats: { title: string, value: number }[] = [];

  constructor(private dashboardService: DashboarderviceService,private router: Router,private jobService:JobService) { }

  ngOnInit(): void {
    this.dashboardService.getDashboard().subscribe(data => {
      this.topStats = [
        { title: 'Total Applications', value: data.totalApplications },
        { title: 'Under Review', value: data.underReviewApplications },
        { title: 'Interviews', value: data.interviewApplications },
        { title: 'Saved Jobs', value: data.savedJobs.length }
      ];

      this.appliedJobs = data.appliedJobs;
      console.log("aplied",this.appliedJobs)
      this.savedJobs = data.savedJobs;
      console.log( "saved",this.savedJobs)
    });

    this.jobService.getRecommendedJobs().subscribe({
    next: (res) => {
      this.recommendedJobs = res;
    },
    error: (err) => {
      console.error(err);
    }
  });
  }
  getStatusClass(status: string): string {
  if (!status) return '';

  switch (status.toLowerCase()) {
    case 'pending':
      return 'status-pending';
    case 'accepted':
    case 'hired':
      return 'status-accepted';
    case 'rejected':
      return 'status-rejected';
    default:
      return '';
  }
}
  goToJobDetails(jobId: number) {
  this.router.navigate(['/details', jobId]);
}

}


