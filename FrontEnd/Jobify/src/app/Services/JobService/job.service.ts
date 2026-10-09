import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { industrieswithjobcount } from '../../Models/Industries/industrieswithjobcount';
import { Observable } from 'rxjs';
import { JobResponse } from '../../Models/Job/jobres';
import { createjob } from '../../Models/Job/createjob';
import { Job } from '../../Models/Job/job';
import { jobdetails } from '../../Models/Job/detailsjob';
import { JobRecommendation } from '../../Models/Job/recommendedjob';
import { OpeningJobs } from '../../Models/Job/openjobsmodel';
import { SearchJob } from '../../Models/Job/search';


@Injectable({
  providedIn: 'root'
})
export class JobService {
private apiUrl = 'https://localhost:7026/api/Job';

   constructor(private HttpClient:HttpClient) { }



  GetAllIndustriesWithJobCount():Observable<industrieswithjobcount[]>{
    return this.HttpClient.get<industrieswithjobcount[]>(`https://localhost:7026/api/Job/GetAllIndustriesWithJobCount`)
  }
  getopeningjobs():Observable<OpeningJobs[]>{
    return this.HttpClient.get<OpeningJobs[]>(`https://localhost:7026/api/Job/OpeningJobs`)

  }



   EmployerJobs():Observable<jobdetails[]>{
    return this.HttpClient.get<jobdetails[]>(`https://localhost:7026/api/Job/my-jobs`)
  }
    JobsForCompanyOwner():Observable<jobdetails[]>{
    return this.HttpClient.get<jobdetails[]>(`https://localhost:7026/api/Job/CompanyJobs`)
  }






  
    getJobs(page: any, pageSize: number): Observable<JobResponse> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    return this.HttpClient.get<JobResponse>(`${this.apiUrl}/search`, { params });
  }




  createJob(job: createjob): Observable<any> {
    return this.HttpClient.post(`${this.apiUrl}/CreateJob`, job);
  }
  
  getJobDetails(id: number) {
  return this.HttpClient.get<jobdetails>(`https://localhost:7026/api/Job/${id}`);
  }



getJobDetailsForJobseeker(jobId: number): Observable<any> {
  return this.HttpClient.get<any>(`https://localhost:7026/api/Job/${jobId}/jobseeker-details`);
}

updateJob(id: number, data: any): Observable<any> {
  return this.HttpClient.put(`https://localhost:7026/api/Job/${id}`, data);
}


  getRecommendedJobs(): Observable<JobRecommendation[]> {
    return this.HttpClient.get<JobRecommendation[]>(`https://localhost:7026/api/Job/recommended`);
  }





search(filter: any) {
  return this.HttpClient.post<any[]>(
    `https://localhost:7026/api/Job/filter`,
    filter
  );
}
// Inside JobService
saveJobAlert(keyword: string): Observable<any> {
  return this.HttpClient.post(`${this.apiUrl}/SaveAlert`, JSON.stringify(keyword), {
    headers: { 'Content-Type': 'application/json' }
  });
}
  getJob(id:number){
    return this.HttpClient.get<Job>(`https://localhost:7026/api/Job/myjob/${id}`);

}

}
