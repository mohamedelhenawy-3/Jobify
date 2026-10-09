import { Component, Input, input } from '@angular/core';
import { JobSeekerApplication } from '../../Models/JobApplications/JobSeekerApplication';
import { JobApplicationService } from '../../Services/JobApplication/jopapplicationservice.service';
import { CommonModule, NgForOf } from '@angular/common';
import { MyApplicationDto } from '../../Models/JobApplications/ApplicationsFilters/MyApplicationDto';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-jobseekerapplications',
  standalone: true,
  imports: [CommonModule,FormsModule,NgForOf],
  templateUrl: './jobseekerapplications.component.html',
  styleUrl: './jobseekerapplications.component.css'
})
export class JobseekerapplicationsComponent {

@Input() data!: MyApplicationDto;
  constructor(private applicationService: JobApplicationService) {}

  // getStatusClass(status: string): string {
  //   return status.toLowerCase().replace(/\s+/g, '');
  // }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Applied':   'status-applied',
      'Interview': 'status-interview',
      'Offer':     'status-offer',
      'Rejected':  'status-rejected',
    };
    return map[status] ?? 'status-applied';
  }

}
