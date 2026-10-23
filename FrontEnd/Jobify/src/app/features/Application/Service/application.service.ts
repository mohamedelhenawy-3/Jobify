import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApplicationDetails, ApplicationStatus, JobApplicationFilter, JobApplicationListItem } from '../models/Application';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private baseUrl = `https://localhost:7026/api/JobApplication`;

  constructor(private http: HttpClient) {}

   private buildParams(filter:JobApplicationFilter){
     let params = new HttpParams();
      if (filter.status !== null && filter.status !== undefined)   
          params =params.set('status',filter.status.toString());
      if (filter.searchTerm?.trim())
          params = params.set('searchTerm', filter.searchTerm.trim());       


      return params;
      }
getApplicationDetails(applicationId: number) {
  return this.http.get<ApplicationDetails>(
    `${this.baseUrl}/${applicationId}`
  );
}
  getSeekerApplications(filter: JobApplicationFilter): Observable<JobApplicationListItem[]> {
    return this.http.get<JobApplicationListItem[]>(`${this.baseUrl}/seeker`,
      { params: this.buildParams(filter) });
  }

  getEmployerApplications(filter: JobApplicationFilter): Observable<JobApplicationListItem[]> {
    return this.http.get<JobApplicationListItem[]>(`${this.baseUrl}/employer`,
      { params: this.buildParams(filter) });
  }


  updateStatus(applicationId: number, body: {
  status: ApplicationStatus;
  interviewDate?: string | null;
  interviewLocation?: string | null;
  notes?: string | null;
}) {
  return this.http.put<{ message: string }>(
    `${this.baseUrl}/${applicationId}/status`,
    body
  );
}
}
