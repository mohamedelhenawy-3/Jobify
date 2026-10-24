// components/join-section/join-section.component.ts
import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthserviceService } from '../../../Services/AuthService/authservice.service';

interface Cta {
  label: string;
  link: string | null;       // null = disabled
  hint?: string;
  hintLink?: { label: string; url: string };
}

@Component({
  selector: 'app-join-section',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './join-section.component.html',
  styleUrl: './join-section.component.css'
})
export class JoinSectionComponent {
  private auth = inject(AuthserviceService);

  readonly seekerPoints = [
    'Create a standout profile',
    'Upload your CV once, apply everywhere',
    'Get instant job alerts',
    'Apply with one click'
  ];

  readonly employerPoints = [
    'Post jobs in minutes',
    'Manage all applicants in one place',
    'Search our candidate database',
    'Build your company branding page'
  ];

  seekerCta = computed<Cta>(() => {
    if (!this.auth.isLoggedIn()) {
      return { label: 'Create your profile', link: '/signup/seeker' };
    }
    if (this.auth.role() === 'JobSeeker') {
      return { label: 'View my profile', link: '/Jobify/profile' };
    }
    return {
      label: 'Create your profile',
      link: null,
      hint: "Employer accounts can't create a job seeker profile."
    };
  });

  employerCta = computed<Cta>(() => {
    if (this.auth.role() === 'Employer') {
      return { label: 'Post a job', link: '/Jobify/post-job' };
    }
    if (!this.auth.isLoggedIn()) {
      return {
        label: 'Post a job',
        link: null,
        hint: 'Sign in with an employer account to post jobs.',
        hintLink: { label: 'Sign in', url: '/login' }
      };
    }
    return {
      label: 'Post a job',
      link: null,
      hint: 'Only employer accounts can post jobs.'
    };
  });
}