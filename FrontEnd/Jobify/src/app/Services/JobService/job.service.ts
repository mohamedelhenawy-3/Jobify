import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { industrieswithjobcount } from '../../Models/Industries/industrieswithjobcount';
import { Observable } from 'rxjs';
import { JobResponse } from '../../Models/Job/jobres';
import { createjob } from '../../Models/Job/createjob';
import { Job } from '../../Models/Job/job';

@Injectable({
  providedIn: 'root'
})
export class JobService {


   constructor(private HttpClient:HttpClient) { }



  GetAllIndustriesWithJobCount():Observable<industrieswithjobcount[]>{
    return this.HttpClient.get<industrieswithjobcount[]>(`https://localhost:44309/api/Job/GetAllIndustriesWithJobCount`)
  }
  
private apiUrl = 'https://localhost:44309/api/Job';


 

  
    getJobs(page: number, pageSize: number, filters: any): Observable<JobResponse> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (filters?.industries?.length) {
      params = params.set('IndustryIds', filters.industries.join(',')); // comma-separated
    }

    if (filters?.types?.length) {
      params = params.set('EmploymentTypeIds', filters.types.join(','));
    }

    if (filters?.modes?.length) {
      params = params.set('WorkModeIds', filters.modes.join(','));
    }

    return this.HttpClient.get<JobResponse>(`${this.apiUrl}/search`, { params });
  }
  createJob(job: createjob): Observable<any> {
    return this.HttpClient.post(`${this.apiUrl}/CreateJob`, job);
  }
    EmployerJobs():Observable<Job[]>{
    return this.HttpClient.get<Job[]>(`https://localhost:44309/api/Job/my-jobs`)
  }
}
