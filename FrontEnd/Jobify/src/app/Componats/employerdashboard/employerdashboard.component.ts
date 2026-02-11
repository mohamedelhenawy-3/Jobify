import { Component, OnInit } from '@angular/core';
import { EmployerdashboarderviceService } from '../../Services/Employerdashboard/employerdashboardervice.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-employerdashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './employerdashboard.component.html',
  styleUrl: './employerdashboard.component.css'
})
export class EmployerdashboardComponent implements OnInit {
  topStats: any = {};
  postedJobs: any[] = [];
  recentApplications: any[] = [];

  constructor(private dashboardService: EmployerdashboarderviceService) {}

  ngOnInit(): void {
    this.dashboardService.getDashboard().subscribe(data => {
      this.topStats = {
        activeJobs: data.activeJobs,
        expiringSoon: data.expiringSoon,
        totalApplications: data.totalApplications,
        newToday: data.newToday
      };
      this.postedJobs = data.postedJobs;
      this.recentApplications = data.recentApplications;
    });
  }

  goToJobDetails(jobId: number) {
    // navigate to job details page (example)
    window.location.href = `/job-details/${jobId}`;
  }
}