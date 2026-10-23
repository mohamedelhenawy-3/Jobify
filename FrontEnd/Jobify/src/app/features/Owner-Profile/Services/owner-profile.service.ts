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
updateProfile(form: UpdateProfileForm, image: File | null) {
  const fd = new FormData();
  if (form.fullName    != null) fd.append('FullName', form.fullName);
  if (form.phoneNumber != null) fd.append('PhoneNumber', form.phoneNumber);
  if (form.location    != null) fd.append('Location', form.location);
  if (form.position    != null) fd.append('Position', form.position);
  if (form.bio         != null) fd.append('Bio', form.bio);
  if (form.linkedIn != null) fd.append('LinkedIn', form.linkedIn);   // add this line
  if (image) fd.append('PImage', image);
  return this.http.put(`${this.baseUrl}/profile`, fd);   // keep your existing URL and method
}

}
