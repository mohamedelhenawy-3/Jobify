import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApplicationService } from '../../Service/application.service';
import {
  ApplicationStatus,
  JobApplicationListItem,
  UpdateAppliedJobStatus
} from '../../models/Application';
import { Router } from '@angular/router';

@Component({
  selector: 'app-employer-applications',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employer-applications.component.html',
  styleUrl: './employer-applications.component.css'
})
export class EmployerApplicationsComponent implements OnInit {

  applications: JobApplicationListItem[] = [];
  loading = false;
  status: ApplicationStatus | null = null;
  searchTerm = '';

  // Used in the template (e.g. Status.Withdrawn)
  Status = ApplicationStatus;

  // Numeric values only: [1, 2, ... 9]
  // (Object.values on a numeric enum also returns the names, so we filter them out)
  filterStatuses = Object.values(ApplicationStatus)
    .filter(v => typeof v === 'number') as ApplicationStatus[];

  // The status that needs date + location. Must match the backend check.
  private readonly INTERVIEW_STATUS = ApplicationStatus.Interviewed;

  // Interview modal state
  selectedApp: JobApplicationListItem | null = null;
  interview = { date: '', location: '', notes: '' };
  errorMsg = '';
  saving = false;

  constructor(private readonly applicationService: ApplicationService, private readonly router: Router) {}

  ngOnInit() {
    this.load();
  }

 load() {
  this.loading = true;
  this.applicationService
    .getEmployerApplications({ status: this.status, searchTerm: this.searchTerm })
    .subscribe({
      next: res => {
        this.applications = res.map(a => ({
          ...a,
          status: this.toStatus(a.status)
        }));
        this.loading = false;
      console.log(res);

      },
      error: () => { this.loading = false; }
    });
}

private toStatus(value: any): ApplicationStatus {
  return typeof value === 'string'
    ? (ApplicationStatus as any)[value]
    : value;
}

label(s: ApplicationStatus): string {
  const name = ApplicationStatus[s];
  return name ? name.replace(/([A-Z])/g, ' $1').trim() : '';
}

  badgeClass(s: ApplicationStatus): string {
    return 'badge-' + this.label(s).toLowerCase().replace(/\s+/g, '-');
  }

  onStatusChange(app: JobApplicationListItem, newStatus: ApplicationStatus) {
    if (newStatus === app.status) return;

    if (newStatus === this.INTERVIEW_STATUS) {
      this.selectedApp = app;
      this.interview = { date: '', location: '', notes: '' };
      this.errorMsg = '';
      return;
    }

    this.sendUpdate(app, { status: newStatus });
  }

  confirmInterview() {
    if (!this.interview.date || !this.interview.location.trim()) {
      this.errorMsg = 'Enter a date and a location.';
      return;
    }

    this.sendUpdate(this.selectedApp!, {
      status: this.INTERVIEW_STATUS,
      interviewDate: this.interview.date,
      interviewLocation: this.interview.location.trim(),
      notes: this.interview.notes.trim() || null
    });
  }

  closeModal() {
    this.selectedApp = null;
    this.errorMsg = '';
    this.load(); // resets the dropdown to the real saved status
  }

  private sendUpdate(app: JobApplicationListItem, body: UpdateAppliedJobStatus) {
    this.saving = true;
    this.applicationService.updateStatus(app.id, body).subscribe({
      next: () => {
        this.saving = false;
        this.selectedApp = null;
        this.errorMsg = '';
        this.load();
      },
      error: err => {
        this.saving = false;
        this.errorMsg = err.error?.message ?? 'Could not update the status. Try again.';
        if (!this.selectedApp) {
          // Error came from a plain dropdown change (no modal open)
          alert(this.errorMsg);
          this.load();
        }
      }
    });
  }

  openDetails(app: JobApplicationListItem) {
  this.router.navigate(['/Jobify/employer-applications-details', app.id]);;
}
}