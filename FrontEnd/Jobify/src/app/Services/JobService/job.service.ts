import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { industrieswithjobcount } from '../../Models/Industries/industrieswithjobcount';
import { Observable } from 'rxjs';
import { JobResponse } from '../../Models/Job/jobres';
import { createjob } from '../../Models/Job/createjob';
import { Job } from '../../Models/Job/job';
import { jobdetails } from '../../Models/Job/dteailsjob';
import { JobRecommendation } from '../../Models/Job/recommendedjob';
import { OpeningJobs } from '../../Models/Job/openjobsmodel';
import { SearchJob } from '../../Models/Job/search';

@Injectable({
  providedIn: 'root'
})
export class JobService {
private apiUrl = 'https://localhost:44309/api/Job';

   constructor(private HttpClient:HttpClient) { }



  GetAllIndustriesWithJobCount():Observable<industrieswithjobcount[]>{
    return this.HttpClient.get<industrieswithjobcount[]>(`https://localhost:44309/api/Job/GetAllIndustriesWithJobCount`)
  }
  getopeningjobs():Observable<OpeningJobs[]>{
    return this.HttpClient.get<OpeningJobs[]>(`https://localhost:44309/api/Job/OpeningJobs`)

  }



 

  
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
    EmployerJobs():Observable<jobdetails[]>{
    return this.HttpClient.get<jobdetails[]>(`https://localhost:44309/api/Job/my-jobs`)
  }
  getJobDetails(id: number) {
  return this.HttpClient.get<jobdetails>(`https://localhost:44309/api/Job/${id}`);
}
getJob(id:number){
    return this.HttpClient.get<Job>(`https://localhost:44309/api/Job/myjob/${id}`);

}
updateJob(id: number, data: any): Observable<any> {
  return this.HttpClient.put(`https://localhost:44309/api/Job/${id}`, data);
}


  getRecommendedJobs(): Observable<JobRecommendation[]> {
    return this.HttpClient.get<JobRecommendation[]>(`https://localhost:44309/api/Job/recommended`);
  }



search(keyword: string) {
  return this.HttpClient.get<SearchJob[]>(
    `https://localhost:44309/api/Job/keyword?keyword=${keyword}`
  );
}

  saveAlert(keyword: string) {
    return this.HttpClient.post('/api/alerts', { keyword });
  }
}
