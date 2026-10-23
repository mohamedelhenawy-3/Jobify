import { Component, OnInit } from '@angular/core';
import { ApplicationStatus, JobApplicationListItem } from '../../models/Application';
import { ApplicationService } from '../../Service/application.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { debounceTime, Subject } from 'rxjs';



@Component({
  selector: 'app-seeker-applications',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './seeker-applications.component.html',
  styleUrls: ['./seeker-applications.component.css']
})
export class SeekerApplicationsComponent implements OnInit {
  applications: JobApplicationListItem[] = [];
  loading = false;
  error = '';

  status: ApplicationStatus | null = null;
  searchTerm = '';

  constructor(private readonly applicationService: ApplicationService) {
  }

    private search$ = new Subject<void>();

  ngOnInit() {
    this.search$.pipe(debounceTime(400)).subscribe(() => this.load());
    this.load();
  }

  onSearchChange() { this.search$.next(); }
   statusOptions = [
    { label: 'All', value: null },
    { label: 'Applied', value: ApplicationStatus.Applied },
    { label: 'UnderReview', value: ApplicationStatus.UnderReview },
    { label: 'Shortlisted', value: ApplicationStatus.Shortlisted },
    { label: 'InterviewScheduled', value: ApplicationStatus.InterviewScheduled },
    { label: 'Interviewed', value: ApplicationStatus.Interviewed },
    { label: 'Offered', value: ApplicationStatus.Offered },
    { label: 'Hired', value: ApplicationStatus.Hired },
    { label: 'Rejected', value: ApplicationStatus.Rejected },
  ];



load() {
  this.loading = true;
  this.error = '';
  this.applicationService
    .getSeekerApplications({ status: this.status, searchTerm: this.searchTerm })
    .subscribe({
      next: res => {
        this.applications = res.map(a => ({ ...a, status: this.toStatus(a.status) }));
        this.loading = false;
      },
      error: err => {
        this.error = err.error?.message ?? 'Failed to load';
        this.loading = false;
      }
    });
}

private toStatus(value: any): ApplicationStatus {
  return typeof value === 'string' ? (ApplicationStatus as any)[value] : value;
}

statusName(s: ApplicationStatus): string {
  return ApplicationStatus[s] ?? '';
}

statusLabel(s: ApplicationStatus): string {
  return this.statusName(s).replace(/([A-Z])/g, ' $1').trim();
}
}
