import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { JobSeekerProfile } from '../../../Models/jobseeker/JobSeekerProfile';
import { JobseekerService } from '../../../Services/JobSeekerservice/jobseeker.service';
import { UpdatePrivacyDto } from '../../../Models/jobseeker/UpdatePrivacy';

@Component({
  selector: 'app-seekerprofile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './seekerprofile.component.html',
  styleUrl: './seekerprofile.component.css'
})
export class SeekerprofileComponent implements OnInit {
privacyToggle: boolean = true; // default, will be synced from backend
  profile!: JobSeekerProfile;
  loading = true;
  editProfile = false;
  isMyProfile = true; 

  skillsInput = '';

  selectedFile: File | null = null;

  // EDUCATIONS
  educations: any[] = [
    {
      institution: '',
      degree: '',
      fieldOfStudy: '',
      startDate: '',
      endDate: ''
    }
  ];

  // EXPERIENCES
  experiences: any[] = [
    {
      companyName: '',
      jobTitle: '',
      description: '',
      startDate: '',
      endDate: ''
    }
  ];

  constructor(private profileService: JobseekerService) {}

  ngOnInit(): void {
    this.profileService.getMyProfile().subscribe({
      next: res => {
        this.profile = res;
        this.privacyToggle = !res.isPrivate;
        console.log("PROFILE", res);
        this.loading = false;

        // load existing education & experience
        if (res.educations) this.educations = res.educations;
        if (res.experiences) this.experiences = res.experiences;
      },
      error: () => this.loading = false
    });
  }

  // IMAGE SELECT
  onFileSelected(event: Event) {

    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }

  }

  // ADD EDUCATION
  addEducation() {

    this.educations.push({
      institution: '',
      degree: '',
      fieldOfStudy: '',
      startDate: '',
      endDate: ''
    });

  }

  // REMOVE EDUCATION
  removeEducation(index: number) {
    this.educations.splice(index, 1);
  }

  // ADD EXPERIENCE
  addExperience() {

    this.experiences.push({
      companyName: '',
      jobTitle: '',
      description: '',
      startDate: '',
      endDate: ''
    });

  }

  // REMOVE EXPERIENCE
  removeExperience(index: number) {
    this.experiences.splice(index, 1);
  }

  // SAVE PROFILE
  saveProfile() {

    const formData = new FormData();

    if (this.profile.fullName)
      formData.append('FullName', this.profile.fullName);

    if (this.profile.headline)
      formData.append('Headline', this.profile.headline);

    if (this.profile.summary)
      formData.append('Summary', this.profile.summary);

    if (this.profile.phoneNumber)
      formData.append('PhoneNumber', this.profile.phoneNumber);

    if (this.profile.location)
      formData.append('Location', this.profile.location);

    if (this.profile.linkedInUrl)
      formData.append('LinkedInUrl', this.profile.linkedInUrl);

    if (this.profile.githubUrl)
      formData.append('GithubUrl', this.profile.githubUrl);

    if (this.selectedFile)
      formData.append('ProfileImage', this.selectedFile);

    // SKILLS
    if (this.skillsInput) {

      const skillsArray = this.skillsInput.split(',').map(s => s.trim());

      skillsArray.forEach(skill => {
        formData.append('Skills', skill);
      });

    }

    // EDUCATIONS JSON
    if (this.educations.length > 0) {

      formData.append(
        'EducationsJson',
        JSON.stringify(this.educations)
      );

    }

    // EXPERIENCES JSON
    if (this.experiences.length > 0) {

      formData.append(
        'ExperiencesJson',
        JSON.stringify(this.experiences)
      );

    }

    this.profileService.updateProfile(formData).subscribe({

      next: () => {

        this.editProfile = false;
        this.selectedFile = null;
        this.skillsInput = '';

        this.ngOnInit();

      },

      error: err => {
        console.error("Update error", err);
      }

    });

  }

  cancelEdit() {

    this.editProfile = false;
    this.selectedFile = null;

  }
  updatePrivacy() {
  const dto: UpdatePrivacyDto = {
    isProfilePublic: this.privacyToggle
  };

  this.profileService.updateProfilePrivacy(dto).subscribe({
    next: res => {
      this.profile.isPrivate = !this.privacyToggle; 
      console.log("private",this.profile.isPrivate )
      alert(res.message);
    },
    error: err => {
      console.error('Failed to update privacy', err);
      this.privacyToggle = !this.privacyToggle;
    }
  });
}
}