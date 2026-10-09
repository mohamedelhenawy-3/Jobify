import { Injectable } from '@angular/core';
import { ActionResult, MyTeamRole, PagedResult, TeamFilter, TeamMember } from '../Models/team';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';

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

   changeRole(id: number, newRole: number) {
    return this.http.put<void>(`${this.baseUrl}/members/${id}/role`, { newRole });
  }
  updatePermissions(id: number, permissions: string[]) {
    return this.http.put<ActionResult>(`${this.baseUrl}/members/${id}/permissions`, { permissions });
  }
  me() {
  return this.http.get<MyTeamRole>(`${this.baseUrl}/me`);
}
setActive(id: number, isActive: boolean) {
  return this.http.put<void>(`${this.baseUrl}/members/${id}/active`, { isActive });
}
remove(id: number) {
  return this.http.delete<ActionResult>(`${this.baseUrl}/${id}`);
}
transferOwnership(id: number) {
  return this.http.post<ActionResult>(`${this.baseUrl}/transfer-ownership/${id}`, {});
}


}
