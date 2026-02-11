import { Component, OnInit } from '@angular/core';
import { AppliedJobSeekerListItem } from '../../Models/JobApplications/AppliedJobSeekers';
import { ApplicationStatus } from '../../Models/JobApplications/ApplicationStatus';
import { JobApplicationService } from '../../Services/JobApplication/jopapplicationservice.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './applications.component.html',
  styleUrls: ['./applications.component.css']
})
export class ApplicationsComponent implements OnInit {
  isLoading = true;
  applications: AppliedJobSeekerListItem[] = [];
  appstatus = ApplicationStatus;
  defaultProfile = 'assets/default-user.png';

  constructor(private applicationservice: JobApplicationService, private router: Router) {}

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.applicationservice.getEmployerApplications()
      .subscribe({
        next: (res) => {
          console.log(res);
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

  changeStatus(applicationId: number, status: ApplicationStatus) {
  this.applicationservice.updateApplicationStatus(applicationId, status)
    .subscribe({
      next: () => {
        console.log(`Application ${applicationId} updated to ${status}`);
      },
      error: (err) => console.error('Failed to update status', err)
    });
}


  viewDetails(applicationId: number): void {
    this.router.navigate(['/ApplicationDetails', applicationId]);
    console.log('View details for application', applicationId);
  }
}
