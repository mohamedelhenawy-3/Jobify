import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { EmployerJobsFilter, JobSearchFilter } from '../models/job-search-filter.model';
import { PagedResult } from '../../../Shared/models/paged-result.model';
import { CreateJobRequest, EmployerJobCard, Job, JobCard, JobDetails, JobMatch } from '../models/Job';
import { SuccessResponse } from '../../../Models/Job/SuccessResponse';
import { EmployerJob } from '../../Owner-Profile/Models/OwnerProfile';


@Injectable({ providedIn: 'root' })
export class JobService {
  private baseUrl = `https://localhost:7026/api/Job`;

  constructor(private http: HttpClient) {}

  searchJobs(filter: JobSearchFilter): Observable<PagedResult<Job>> {
    let params = new HttpParams();

    Object.entries(filter).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, value.toString());
      }
    });

    return this.http.get<PagedResult<Job>>(`${this.baseUrl}/search`, { params });
  }
    createJob(request: CreateJobRequest): Observable<SuccessResponse> {
    return this.http.post<SuccessResponse>(`${this.baseUrl}/CreateJob`, request);
  }
  getJobById(id: number): Observable<JobDetails> {
  return this.http.get<JobDetails>(`${this.baseUrl}/${id}/detailsJobSeeker-Job`);
}
  applyToJob(jobId: number, application: FormData): Observable<any> {
  return this.http.post(`${this.baseUrl}/CreateJobApplication/${jobId}`, application);
  }

  getMyJobs(): Observable<EmployerJob[]> {
  return this.http.get<EmployerJob[]>(`${this.baseUrl}/EmpJobs`);
}

private toParams(filter: object): HttpParams {
  let params = new HttpParams();
  Object.entries(filter).forEach(([key, value]) => {
    if (value !== null && value !== undefined && value !== '') {
      params = params.set(key, String(value));
    }
  });
  return params;
}

getEmployerJobs(filter: EmployerJobsFilter) {
  return this.http.get<PagedResult<JobCard>>(`${this.baseUrl}/EmployerJobs`, { params: this.toParams(filter) });
}

getJobs(filter: EmployerJobsFilter) {
  return this.http.get<PagedResult<JobCard>>(`${this.baseUrl}/Jobs`, { params: this.toParams(filter) });
}
getSimilar(id: number, take = 5) {
  return this.http.get<JobMatch[]>(`${this.baseUrl}/${id}/similar?take=${take}`);
}
}