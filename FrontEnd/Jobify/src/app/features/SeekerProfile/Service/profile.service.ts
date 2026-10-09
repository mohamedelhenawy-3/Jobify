import { inject, Injectable } from '@angular/core';
import { ProfileDto } from '../Models/seeker';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

 private http = inject(HttpClient);
 
 private base = `https://localhost:7026/api/JobSeeker`;


  me() { return this.http.get<ProfileDto>(this.base + '/me'); }
  updateBasic(d: any) { return this.http.put<ProfileDto>(this.base + '/basic', d); }
  updateAbout(about: string) { return this.http.put<ProfileDto>(this.base + '/about', { about }); }
  saveSkills(skills: string[]) { return this.http.put<ProfileDto>(this.base + '/skills', { skills }); }

  uploadPhoto(file: File) {
    const fd = new FormData();
    fd.append('ProfileImage', file);
    return this.http.post<ProfileDto>(this.base + '/photo', fd);
  }
  uploadCv(file: File) {
    const fd = new FormData();
    fd.append('Resume', file);
    return this.http.post<ProfileDto>(this.base + '/cv', fd);
  }
  removeCv() { return this.http.delete<ProfileDto>(this.base + '/cv'); }
 saveExperience(id: number | null, d: any) {
  return id
    ? this.http.put<ProfileDto>(this.base + '/experiences/' + id, d)
    : this.http.post<ProfileDto>(this.base + '/experiences', d);
}
deleteExperience(id: number) {
  return this.http.delete<ProfileDto>(this.base + '/experiences/' + id);
}

saveEducation(id: number | null, d: any) {
  return id
    ? this.http.put<ProfileDto>(this.base + '/educations/' + id, d)
    : this.http.post<ProfileDto>(this.base + '/educations', d);
}
deleteEducation(id: number) {
  return this.http.delete<ProfileDto>(this.base + '/educations/' + id);
}
savePreferences(d: any) {
  return this.http.put<ProfileDto>(this.base + '/preferences', d);
}
}
