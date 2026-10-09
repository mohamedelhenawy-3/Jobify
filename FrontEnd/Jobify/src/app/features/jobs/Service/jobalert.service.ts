import { Injectable } from '@angular/core';
import { JobAlert } from '../models/Job';
import { ActionResult } from '../../Team/Models/team';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class JobalertService {
  private baseUrl = `https://localhost:7026/api/JobAlerts`;
    constructor(private http: HttpClient) {}

  create(keyword: string) { return this.http.post<ActionResult>(this.baseUrl, { keyword }); }
  mine()                  { return this.http.get<JobAlert[]>(this.baseUrl); }
  delete(id: number)      { return this.http.delete<ActionResult>(`${this.baseUrl}/${id}`); }
  setActive(id: number, isActive: boolean) {
    return this.http.put<ActionResult>(`${this.baseUrl}/${id}/active`, { isActive });
  }
}
