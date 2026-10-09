import { Component, OnInit } from '@angular/core';
import { ApplicationService } from '../../Service/application.service';
import { ActivatedRoute } from '@angular/router';
import { ApplicationDetails, ApplicationStatus } from '../../models/Application';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-application-details',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './application-details.component.html',
  styleUrl: './application-details.component.css'
})
export class ApplicationDetailsComponent implements OnInit {
 details: ApplicationDetails | null = null;
  loading = true;
  error = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly applicationService: ApplicationService
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.applicationService.getApplicationDetails(id).subscribe({
      next: res => {
        this.details = { ...res, status: this.toStatus(res.status) };
        this.loading = false;
      },
      error: err => {
        this.error = err.error?.message ?? 'Could not load this application.';
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
