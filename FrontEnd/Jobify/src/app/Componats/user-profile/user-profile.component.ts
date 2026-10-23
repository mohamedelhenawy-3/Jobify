import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../Services/User/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css'
})
export class UserProfileComponent {
  profile!: any;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('id');
    if (userId) {
      this.userService.getUserProfile(userId).subscribe({
        next: (res) => {
          this.profile = res;
          this.loading = false;
          console.log(this.profile);
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        },
      });
    }
  }

  viewCompanyJobs() {
    if (this.profile.userRole === 'Employer' && this.profile.company) {
      window.location.href = `/Jobify/managejobs/${this.profile.id}`;
  }
}
}
