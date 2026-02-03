import { Component, OnInit } from '@angular/core';
import { AppliedJobSeekerListItem } from '../../Models/JobApplications/AppliedJobSeekers';
import { ApplicationStatus } from '../../Models/JobApplications/ApplicationStatus';
import { JobApplicationService } from '../../Services/JobApplication/jopapplicationservice.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './applications.component.html',
  styleUrl: './applications.component.css'
})
export class ApplicationsComponent implements OnInit{
    isLoading = true;
  constructor(private applicationservice: JobApplicationService,private router:Router) {}
  ngOnInit(): void {
    this.loadApplications();
  }
  applications: AppliedJobSeekerListItem[] = [];
  appstatus=ApplicationStatus; 

    loadApplications(): void {
    this.applicationservice.getEmployerApplications()
      .subscribe({
        next: (res) => {
          console.log(res)
          this.applications = res;
          this.isLoading = false;
        },
        error: () => this.isLoading = false
      });
  }
    getStatusLabel(status: ApplicationStatus): string {
    return ApplicationStatus[status];
  }

  statusClass(status: ApplicationStatus): string {
    switch (status) {
      case ApplicationStatus.Applied: return 'badge bg-secondary';
      case ApplicationStatus.UnderReview: return 'badge bg-info';
      case ApplicationStatus.Shortlisted: return 'badge bg-primary';
      case ApplicationStatus.InterviewScheduled: return 'badge bg-warning';
      case ApplicationStatus.Offered: return 'badge bg-success';
      case ApplicationStatus.Rejected: return 'badge bg-danger';
      default: return 'badge bg-dark';
    }
  }

viewDetails(applicationId: number): void {
  // Navigate to details page
  this.router.navigate(['/ApplicationDetails', applicationId]);

  console.log('View details for application', applicationId);
}

}
