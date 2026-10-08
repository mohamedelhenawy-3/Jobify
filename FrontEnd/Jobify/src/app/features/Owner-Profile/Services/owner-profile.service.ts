import { Injectable } from '@angular/core';
import { EmployerJob, EmployerProfileDto, RecentApplicant, UpdateProfileForm } from '../Models/OwnerProfile';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class OwnerProfileService {
 private baseUrl = `https://localhost:7026/api/Employer`;
 constructor(private http: HttpClient) {}



getOwnerProfile(): Observable<EmployerProfileDto> {
    return this.http.get<EmployerProfileDto>(`https://localhost:7026/api/Employer/profile`);
  }
  updateProfile(form: UpdateProfileForm, image?: File | null): Observable<{ message: string }> {
  const fd = new FormData();
  if (form.fullName)    fd.append('FullName', form.fullName);
  if (form.phoneNumber) fd.append('PhoneNumber', form.phoneNumber);
  if (form.location)    fd.append('Location', form.location);
  // if (form.linkedInUrl) fd.append('LinkedInUrl', form.linkedInUrl);
  if (form.position)    fd.append('Position', form.position);
  if (form.bio)         fd.append('Bio', form.bio);
  if (image)            fd.append('PImage', image);

  return this.http.put<{ message: string }>(`${this.baseUrl}/profile`, fd);
}


}
