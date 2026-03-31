import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JobSeekerProfile } from '../../Models/jobseeker/JobSeekerProfile';
import { EmployerProfileResponseDto } from '../../Models/Employer/EmployerProfileResponse';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmployerService {

  private baseUrl = 'https://localhost:44309/api';

  constructor(private http: HttpClient) {}

  // 🔹 Get logged-in user's profile
  getMyProfile(): Observable<EmployerProfileResponseDto> {
    return this.http.get<EmployerProfileResponseDto>(`${this.baseUrl}/Employer`);
  }
   updateProfile(formData: FormData): Observable<any> {
    return this.http.put(`${this.baseUrl}/me`, formData);
  }
}
