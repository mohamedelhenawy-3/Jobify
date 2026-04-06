import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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
resetFilters() {
throw new Error('Method not implemented.');
}


  @Input() applications: any[] = [];
  @Input() isLoading = false;
  @Output() onStatusUpdated = new EventEmitter<void>();
  appStatus = ApplicationStatus;
  defaultProfile = 'assets/default-user.png';

  // State for the conditional Interview form
  // appStatus = ApplicationStatus;
  editingInterviewId: number | null = null;
  interviewForm: UpdateStatus = this.getInitialFormState();
  constructor(
    private applicationService: JobApplicationService, 
    private router: Router
  ) {}

  ngOnInit(): void {
    // this.loadApplications();
  }

  loadApplications(): void {
    this.isLoading = true;
    this.applicationService.getEmployerApplications().subscribe({
      next: (res) => {
        this.applications = res;
        console.log("application loaded",this.applications);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Fetch Error:', err);
        this.isLoading = false;
      }
    });
  }


 onStatusChange(applicationId: number, newStatus: string): void {
    if (newStatus === 'Interviewed') {
      this.editingInterviewId = applicationId;
      this.interviewForm = this.getInitialFormState();
    } else {
      this.editingInterviewId = null;
      this.saveStatus(applicationId, { status: newStatus as ApplicationStatus });
    }
  }

saveStatus(applicationId: number, data: UpdateStatus) {
    const payload: any = {
      status: data.status,
      interviewLocation: data.interviewLocation || null,
      notes: data.notes || null,
      interviewDate: data.interviewDate ? new Date(data.interviewDate).toISOString().split('.')[0] : null
    };

    this.applicationService.updateApplicationStatus(applicationId, payload)
      .subscribe({
        next: () => {
          this.editingInterviewId = null; 
          this.onStatusUpdated.emit(); // 3. Notify Parent to refresh list
          alert("Status updated!");
        },
        error: (err) => alert("Error: " + JSON.stringify(err.error))
      });
  }

 private getInitialFormState(): UpdateStatus {
    return { status: ApplicationStatus.Interviewed, interviewDate: '', interviewLocation: '', notes: '' };
  }

  viewDetails(applicationId: number): void {
    this.router.navigate(['/Jobify/ApplicationDetails', applicationId]);
  }
}