import { Injectable } from '@angular/core';
import { ActionResult, ActivityItem, MyTeamRole, PagedResult, RoleName, TeamFilter, TeamMember } from '../Models/team';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivityDto, ActivityFilter } from '../../Activity/avrvity';

// body for remove / leave: who takes over the jobs (optional)
export interface RemoveMemberRequest {
  reassignToEmployerId?: number | null;
}

@Injectable({
  providedIn: 'root'
})
export class TeamService {

  private baseUrl = 'https://localhost:7026/api/CompanyMembers';   // use your real API URL

  constructor(private http: HttpClient) {}

  getMembers(f: TeamFilter): Observable<PagedResult<TeamMember>> {
    let params = new HttpParams()
      .set('pageNumber', f.pageNumber)
      .set('pageSize', f.pageSize);

    if (f.search)           params = params.set('search', f.search);
    if (f.role != null)     params = params.set('role', f.role);
    if (f.isActive != null) params = params.set('isActive', f.isActive);
    if (f.sortBy)           params = params.set('sortBy', f.sortBy);

    return this.http.get<PagedResult<TeamMember>>(`${this.baseUrl}/membersfilters`, { params });
  }

 changeRole(id: number, newRole: RoleName) {
  return this.http.put<void>(`${this.baseUrl}/members/${id}/role`, { newRole });
}
  updatePermissions(id: number, permissions: string[]) {
    return this.http.put<ActionResult>(`${this.baseUrl}/members/${id}/permissions`, { permissions });
  }

  me() {
    return this.http.get<MyTeamRole>(`${this.baseUrl}/me`);
  }

  getActivity(f: { type?: string; pageNumber: number; pageSize: number }): Observable<PagedResult<ActivityItem>> {
  let params = new HttpParams()
    .set('pageNumber', f.pageNumber)
    .set('pageSize', f.pageSize);
  if (f.type) params = params.set('type', f.type);
  return this.http.get<PagedResult<ActivityItem>>(`${this.baseUrl}/activity`, { params });
}

  setActive(id: number, hasAccessed: boolean) {
    return this.http.put<void>(`${this.baseUrl}/members/${id}/active`, { hasAccessed });
  }
  getMyActivity(f: ActivityFilter): Observable<PagedResult<ActivityDto>> {
    return this.http.get<PagedResult<ActivityDto>>(`${this.baseUrl}/activity/me`, { params: this.toParams(f) });
  }
 
  // For later (Owner/Admin only)
  getCompanyActivity(f: ActivityFilter): Observable<PagedResult<ActivityDto>> {
    return this.http.get<PagedResult<ActivityDto>>(`${this.baseUrl}/companyactivity`, { params: this.toParams(f) });
  }
   private toParams(f: ActivityFilter): HttpParams {
    let p = new HttpParams()
      .set('PageNumber', f.pageNumber)
      .set('PageSize', f.pageSize);
    if (f.search.trim()) p = p.set('Search', f.search.trim());
    if (f.category)      p = p.set('Category', f.category);
    if (f.from)          p = p.set('From', f.from);
    if (f.to)            p = p.set('To', f.to);
    return p;
  }
 
  // DELETE with a body: Angular needs the `body` option
  remove(id: number, reassignToEmployerId?: number | null) {
    const body: RemoveMemberRequest = { reassignToEmployerId: reassignToEmployerId ?? null };
    return this.http.delete<ActionResult>(`${this.baseUrl}/members/${id}`, { body });
  }

  // current user leaves the company
  leave(reassignToEmployerId?: number | null) {
    const body: RemoveMemberRequest = { reassignToEmployerId: reassignToEmployerId ?? null };
    return this.http.post<ActionResult>(`${this.baseUrl}/members/leave`, body);
  }

  transferOwnership(id: number) {
    return this.http.post<ActionResult>(`${this.baseUrl}/transfer-ownership/${id}`, {});
  }

  // helper: read the backend message from an error response
  static errorMessage(err: any): string {
    return err?.error?.message ?? err?.error?.title ?? 'Something went wrong';
  }
}