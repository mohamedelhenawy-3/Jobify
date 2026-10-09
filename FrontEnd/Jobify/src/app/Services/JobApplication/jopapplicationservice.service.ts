import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApplicationStatus } from '../../Models/JobApplications/ApplicationStatus';
import { AppliedJobSeekerListItem } from '../../Models/JobApplications/AppliedJobSeekers';
import { AppliedJobSeekerDetails } from '../../Models/JobApplications/appliedSeekerDetails';
import { CreateJobApplication } from '../../Models/JobApplications/CreateJobApplication';
import { JobSeekerApplication } from '../../Models/JobApplications/JobSeekerApplication';
import { UpdateStatus } from '../../Models/JobApplications/UpdateStatus';
import { ApplicationFilter } from '../../Models/JobApplications/ApplicationsFilters/jobappfilters';
import { MyApplicationDto } from '../../Models/JobApplications/ApplicationsFilters/MyApplicationDto';
import { HiringPipelineDto, RecentApplicant } from '../../features/Owner-Profile/Models/OwnerProfile';

@Injectable({
  providedIn: 'root'
})
export class JobApplicationService {

  private baseUrl = 'https://localhost:7026/api/JobApplication';
  // change URL to your real API

  constructor(private http: HttpClient) {}






  getPipeline(): Observable<HiringPipelineDto> {
    return this.http.get<HiringPipelineDto>(`${this.baseUrl}/pipeline`);
  }

  getRecentApplicants(take = 5): Observable<RecentApplicant[]> {
    const params = new HttpParams().set('take', take);
    return this.http.get<RecentApplicant[]>(`${this.baseUrl}/recent-applicants`, { params });
  }

  getEmployerApplications(): Observable<AppliedJobSeekerListItem[]> {
    return this.http.get<AppliedJobSeekerListItem []>(`${this.baseUrl}`);
  }

  getApplicationDetails(applicationId: number): Observable<AppliedJobSeekerDetails> {
    return this.http.get<AppliedJobSeekerDetails>(`${this.baseUrl}/AppliedJobDetails/${applicationId}`);
  }
  applyToJob(jobId: number, application: FormData): Observable<any> {
  return this.http.post(`${this.baseUrl}/CreateJobApplication/${jobId}`, application);
  }

  getMyApplications(): Observable<JobSeekerApplication[]> {
    return this.http.get<JobSeekerApplication[]>(`${this.baseUrl}/MyApplications`);
  }

  updateApplicationStatus(applicationId: number, status: UpdateStatus): Observable<any> {
     return this.http.put(`${this.baseUrl}/AppliedJobDetails/${applicationId}`, status);
   }

  MyApplications(filter: ApplicationFilter): Observable<MyApplicationDto[]> {
      const params=new HttpParams().set('status',filter.status);
      if(filter.search){
        params.set('search',filter.search);
      }

      return this.http.get<MyApplicationDto[]>(`${this.baseUrl}/my-applications`,{params});
}
getEmpApplications(filter: ApplicationFilter): Observable<AppliedJobSeekerListItem[]> {
  let params = new HttpParams().set('status', filter.status);
  
  if (filter.search) {
    params = params.set('search', filter.search);
  }

  return this.http.get<AppliedJobSeekerListItem[]>(`${this.baseUrl}/appliedseekers`, { params });
}
}