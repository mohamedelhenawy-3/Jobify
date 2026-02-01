import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { JobSeekerProfile } from '../../../Models/jobseeker/JobSeekerProfile';
import { JobseekerService } from '../../../Services/JobSeekerservice/jobseeker.service';

@Component({
  selector: 'app-seekerprofile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './seekerprofile.component.html',
  styleUrl: './seekerprofile.component.css'
})
export class SeekerprofileComponent implements OnInit {

  profile!: JobSeekerProfile;
  loading = true;
  editProfile = false;
skillsInput = ''; // string of skills comma separated

  selectedFile: File | null = null;

  constructor(
    private profileService: JobseekerService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.profileService.getMyProfile().subscribe({
      next: res => {
        this.profile = res;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  // ✅ IMAGE SELECTION
  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

saveProfile() {
  const formData = new FormData();

  if (this.profile.headline) formData.append('Headline', this.profile.headline);
  if (this.profile.summary) formData.append('Summary', this.profile.summary);
  if (this.profile.phoneNumber) formData.append('PhoneNumber', this.profile.phoneNumber);
  if (this.profile.location) formData.append('Location', this.profile.location);
  if (this.selectedFile) formData.append('ProfileImage', this.selectedFile);

  // ✅ Add skills
  if (this.skillsInput) {
const skillsArray = this.skillsInput.split(',').map(s => s.trim());
skillsArray.forEach(skill => formData.append('Skills', skill));
  }

  this.http.put('https://localhost:44309/api/JobSeeker/me', formData)
    .subscribe(() => {
      this.editProfile = false;
      this.selectedFile = null;
      this.skillsInput = '';
      // optionally reload profile to see updated skills
      this.ngOnInit();
    });
}


  cancelEdit() {
    this.editProfile = false;
    this.selectedFile = null;
  }
}
