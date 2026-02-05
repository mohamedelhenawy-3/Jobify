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
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  getStatusClass(status: string): string {
    return status.toLowerCase();
  }
}
