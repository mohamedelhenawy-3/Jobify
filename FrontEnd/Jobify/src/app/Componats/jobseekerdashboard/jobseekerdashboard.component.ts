import { Component, OnInit } from '@angular/core';
import { DashboarderviceService } from '../../Services/DashBoardService/dashboardervice.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-jobseekerdashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './jobseekerdashboard.component.html',
  styleUrl: './jobseekerdashboard.component.css'
})
export class JobseekerdashboardComponent  implements OnInit{
   appliedJobs: any[] = [];
  savedJobs: any[] = [];

  topStats: { title: string, value: number }[] = [];

  constructor(private dashboardService: DashboarderviceService) { }

  ngOnInit(): void {
    this.dashboardService.getDashboard().subscribe(data => {
      this.topStats = [
        { title: 'Total Applications', value: data.totalApplications },
        { title: 'Under Review', value: data.underReviewApplications },
        { title: 'Interviews', value: data.interviewApplications },
        { title: 'Pending Applications', value: data.pendingApplications },
        { title: 'Saved Jobs', value: data.savedJobs.length }
      ];

      this.appliedJobs = data.appliedJobs;
      this.savedJobs = data.savedJobs;
    });
  }
}


