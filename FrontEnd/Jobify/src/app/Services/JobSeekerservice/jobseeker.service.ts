import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JobSeekerProfile } from '../../Models/jobseeker/JobSeekerProfile';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class JobseekerService {

  private baseUrl = 'https://localhost:44309/api/JobSeeker';

  constructor(private http: HttpClient) {}

  // 🔹 Get logged-in user's profile
  getMyProfile(): Observable<JobSeekerProfile> {
    return this.http.get<JobSeekerProfile>(`${this.baseUrl}/me`);
  }
}
