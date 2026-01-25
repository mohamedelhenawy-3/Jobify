import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [],
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css'] // ✅ fixed
})
export class LandingPageComponent {

  constructor(private router: Router) {} // ✅ Angular Router

  navigateTo(role: string) {
    if (role === 'job-seeker') this.router.navigate(['/register/job-seeker']);
    if (role === 'employer') this.router.navigate(['/register/employer']);
  }
}
