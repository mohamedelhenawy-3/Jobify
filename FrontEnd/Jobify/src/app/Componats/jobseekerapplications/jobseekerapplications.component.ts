import { Component } from '@angular/core';
import { JobSeekerApplication } from '../../Models/JobApplications/JobSeekerApplication';
import { JobApplicationService } from '../../Services/JobApplication/jopapplicationservice.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-jobseekerapplications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './jobseekerapplications.component.html',
  styleUrl: './jobseekerapplications.component.css'
})
export class JobseekerapplicationsComponent {
  applications: JobSeekerApplication[] = [];
  loading = true;

  constructor(private applicationService: JobApplicationService) {}

  ngOnInit(): void {
    this.applicationService.getMyApplications().subscribe({
      next: (res) => {
        console.log(res);
        this.applications = res;
        console.log("my applications",res);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
// Maps status to CSS classes for colors
  getStatusClass(status: string): string {
    return status.toLowerCase().replace(/\s+/g, '');
  }

  // Counts for the summary cards
  getCount(status: string): number {
    return this.applications.filter(app => 
      app.status.toLowerCase() === status.toLowerCase()
    ).length;
  }

  // Generates a consistent background color for the logo fallback based on company name
  getLogoBgColor(name: string): string {
    const colors = ['#DBEAFE', '#D1FAE5', '#FEE2E2', '#FEF3C7', '#F3E8FF', '#E0F2FE'];
    let hash = 0;
    for (let i = 0; i < name.length; i++) {
      hash = name.charCodeAt(i) + ((hash << 5) - hash);
    }
    return colors[Math.abs(hash) % colors.length];
  }

}
