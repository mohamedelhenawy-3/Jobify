import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AppliedJobSeekerListItem } from '../../Models/JobApplications/AppliedJobSeekers';
import { ApplicationStatus } from '../../Models/JobApplications/ApplicationStatus';
import { JobApplicationService } from '../../Services/JobApplication/jopapplicationservice.service';
import { UpdateStatus } from '../../Models/JobApplications/UpdateStatus';

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
  appStatus = ApplicationStatus;
  defaultProfile = 'assets/default-user.png';

  // State for the conditional Interview form
  editingInterviewId: number | null = null;
  interviewForm: UpdateStatus = this.getInitialFormState();

  constructor(
    private applicationService: JobApplicationService, 
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.isLoading = true;
    this.applicationService.getEmployerApplications().subscribe({
      next: (res) => {
        this.applications = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Fetch Error:', err);
        this.isLoading = false;
      }
    });
  }

  // Logic to handle dropdown changes
  onStatusChange(applicationId: number, newStatus: string): void {
    if (newStatus === 'Interviewed') {
      // Open the form for this specific candidate
      this.editingInterviewId = applicationId;
      this.interviewForm = this.getInitialFormState();
    } else {
      // Immediate update for any other status
      this.editingInterviewId = null;
      const quickUpdate: UpdateStatus = { status: newStatus as ApplicationStatus };
      this.saveStatus(applicationId, quickUpdate);
    }
  }

  // Logic to send data to C# API
saveStatus(applicationId: number, data: UpdateStatus) {
  // 1. Create a clean payload to send to the API
  const payload: any = {
    status: data.status,
    interviewLocation: data.interviewLocation || null,
    notes: data.notes || null,
    // 2. Ensure Date is in the format: YYYY-MM-DDTHH:mm:ss
    interviewDate: data.interviewDate ? new Date(data.interviewDate).toISOString().split('.')[0] : null
  };

  console.log('Sending Payload:', payload); // Check this in your console!

  this.applicationService.updateApplicationStatus(applicationId, payload)
    .subscribe({
      next: (res) => {
        this.editingInterviewId = null; 
        this.loadApplications(); 
        alert("Status updated to Interviewed!");
      },
      error: (err) => {
        console.error('API Error details:', err.error);
        // This alert will show you EXACTLY why the backend rejected it
        alert("Backend Error: " + JSON.stringify(err.error?.errors || err.error));
      }
    });
}

  private getInitialFormState(): UpdateStatus {
    return {
      status: ApplicationStatus.Interviewed,
      interviewDate: '',
      interviewLocation: '',
      notes: ''
    };
  }

  viewDetails(applicationId: number): void {
    this.router.navigate(['/Jobify/ApplicationDetails', applicationId]);
  }
}