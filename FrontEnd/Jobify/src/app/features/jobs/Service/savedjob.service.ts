import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { SavedJob } from '../models/saved-job';
import { PagedResult } from '../models/Job';


@Injectable({ providedIn: 'root' })
export class SavedJobService {
  private base = `https://localhost:7026/api/SavedJob`;
  constructor(private http: HttpClient) {}

  isSaved(jobId: number) { return this.http.get<{ isSaved: boolean }>(`${this.base}/${jobId}/status`); }
  save(jobId: number)    { return this.http.put<void>(`${this.base}/${jobId}`, {}); }
  unsave(jobId: number)  { return this.http.delete<void>(`${this.base}/${jobId}`); }
  list(pageNumber: number, pageSize = 10) {
    const params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    return this.http.get<PagedResult<SavedJob>>(this.base, { params });
  }
}