import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JoinRequestFilter, JoinRequestItem } from '../../Models/join-request';
import { PagedResult } from '../../Models/team';

@Injectable({
  providedIn: 'root'
})
export class JoinRequestServiceService {

  private base = 'https://localhost:7026/api/Onboarding';   // your controller route
  constructor(private http: HttpClient) {}

  list(f: JoinRequestFilter) {
    const params = new HttpParams()
      .set('status', f.status)
      .set('pageNumber', f.pageNumber)
      .set('pageSize', f.pageSize);
    return this.http.get<PagedResult<JoinRequestItem>>(`${this.base}/join-requests`, { params });
  }
 process(requestId: number, isApproved: boolean) {
    return this.http.post<void>(`${this.base}/process-join-request`, { requestId, isApproved });
}

}
