import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JobSeekerProfile } from '../../Models/jobseeker/JobSeekerProfile';
import { Observable } from 'rxjs';
import { UpdatePrivacyDto } from '../../Models/jobseeker/UpdatePrivacy';

@Injectable({
  providedIn: 'root'
})
export class JobseekerService {

  private baseUrl = 'https://localhost:7026/api/JobSeeker';

  constructor(private http: HttpClient) {}

  // 🔹 Get logged-in user's profile
  getMyProfile(): Observable<JobSeekerProfile> {
    return this.http.get<JobSeekerProfile>(`${this.baseUrl}/me`);
  }
   updateProfile(formData: FormData): Observable<any> {
    return this.http.put(`${this.baseUrl}/me`, formData);
  }
  updateProfilePrivacy(dto: UpdatePrivacyDto) {
  return this.http.put<{ message: string }>('https://localhost:7026/api/JobSeeker/privacy', dto);
}
}
