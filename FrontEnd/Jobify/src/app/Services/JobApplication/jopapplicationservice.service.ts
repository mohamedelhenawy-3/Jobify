import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApplicationStatus } from '../../Models/JobApplications/ApplicationStatus';
import { AppliedJobSeekerListItem } from '../../Models/JobApplications/AppliedJobSeekers';
import { AppliedJobSeekerDetails } from '../../Models/JobApplications/appliedSeekerDetails';
import { CreateJobApplication } from '../../Models/JobApplications/CreateJobApplication';
import { JobSeekerApplication } from '../../Models/JobApplications/JobSeekerApplication';


@Injectable({
  providedIn: 'root'
})
export class JobApplicationService {

  private baseUrl = 'https://localhost:44309/api/JobApplication';
  // change URL to your real API

  constructor(private http: HttpClient) {}

  /* ============================
     Employer
     ============================ */

  // Get all applications for employer jobs
  getEmployerApplications(): Observable<AppliedJobSeekerListItem[]> {
    return this.http.get<AppliedJobSeekerListItem []>(`${this.baseUrl}`);
  }

  // Get details of a specific application
  getApplicationDetails(applicationId: number): Observable<AppliedJobSeekerDetails> {
    return this.http.get<AppliedJobSeekerDetails>(`${this.baseUrl}/AppliedJobDetails/${applicationId}`);
  }
 applyToJob(jobId: number, application: CreateJobApplication): Observable<any> {
    return this.http.post(`${this.baseUrl}/CreateJobApplication/${jobId}`, application);
  }
  // Update application status
  // updateApplicationStatus(
  //   applicationId: number,
  //   status: ApplicationStatus
  // ): Observable<any> {
  //   return this.http.put(`${this.baseUrl}/AppliedJobDetails/${applicationId}`, {
  //     status: status
  //   });
  // }

  /* ============================
     Job Seeker
     ============================ */

  // // Get my applications (job seeker)
  getMyApplications(): Observable<JobSeekerApplication[]> {
    return this.http.get<JobSeekerApplication[]>(`${this.baseUrl}/MyApplications`);
  }

  // // Withdraw application
  // withdrawApplication(applicationId: number): Observable<any> {
  //   return this.http.put(`${this.baseUrl}/${applicationId}/withdraw`, {});
  // }
    updateApplicationStatus(applicationId: number, status: ApplicationStatus): Observable<any> {
    return this.http.put(`${this.baseUrl}/AppliedJobDetails/${applicationId}`, { status });
  }
}
