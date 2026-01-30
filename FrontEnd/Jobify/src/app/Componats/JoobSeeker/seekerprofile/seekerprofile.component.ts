import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { JobSeekerProfile } from '../../../Models/jobseeker/JobSeekerProfile';
import { JobseekerService } from '../../../Services/JobSeekerservice/jobseeker.service';

@Component({
  selector: 'app-seekerprofile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './seekerprofile.component.html',
  styleUrl: './seekerprofile.component.css'
})
export class SeekerprofileComponent  implements OnInit{
  profile!: JobSeekerProfile;
  loading = true;

  constructor(private profileService: JobseekerService) {}

  ngOnInit(): void {
    this.profileService.getMyProfile().subscribe({
      next: res => {
        this.profile = res;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }
}
