import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SavedjobsService  {
private apiUrl = 'https://localhost:7026/api/SavedJob';
    constructor(private HttpClient:HttpClient) { }
  getSavedJobs() {
    return this.HttpClient.get(`${this.apiUrl}`);
  }


  saveJob(jobId: number): Observable<void> {
    return this.HttpClient.post<void>(`${this.apiUrl}/${jobId}`, {});
  }

  unsaveJob(jobId: number): Observable<void> {
    return this.HttpClient.delete<void>(`${this.apiUrl}/${jobId}`);
  }



  isJobSaved(jobId: number): Observable<boolean> {
    return this.HttpClient.get<boolean>(`${this.apiUrl}/check/${jobId}`);
  }

 




}
