import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SavedjobsService  {
private apiUrl = 'https://localhost:44309/api/SavedJob';
    constructor(private HttpClient:HttpClient) { }
  getSavedJobs() {
    return this.HttpClient.get(`${this.apiUrl}`);
  }

  saveJob(jobId: string) {
    return this.HttpClient.post(`${this.apiUrl}/save`, { jobId });
  }

  removeSavedJob(jobId: string) {
    return this.HttpClient.delete(`${this.apiUrl}/saved/${jobId}`);
  }

 




}
