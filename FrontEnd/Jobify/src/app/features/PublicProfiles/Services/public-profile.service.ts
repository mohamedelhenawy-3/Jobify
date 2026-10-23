import { inject, Injectable } from '@angular/core';
import { PublicCompany, PublicProfile } from '../Models/profiles';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PublicProfileService {

  private http = inject(HttpClient);
  private base = `https://localhost:7026/api/PublicProfiles`;
 
 
  getProfile(userId: string): Observable<PublicProfile> {
    return this.http.get<PublicProfile>(`${this.base}/PublicProfile/${userId}`);
  }

  getCompany(id: number, maxJobs = 10): Observable<PublicCompany> {
    return this.http.get<PublicCompany>(`${this.base}/companies/${id}?maxJobs=${maxJobs}`);
  }
}
