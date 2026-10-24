import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

interface FooterLink {
  label: string;
  url: string;
}

interface FooterColumn {
  title: string;
  links: FooterLink[];
}

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent {
  year = new Date().getFullYear();

  columns: FooterColumn[] = [
    {
      title: 'For Job Seekers',
      links: [
        { label: 'Browse jobs', url: '/jobs' },
        { label: 'Companies', url: '/companies' },
        { label: 'Create profile', url: '/signup/seeker' },
        { label: 'Saved jobs', url: '/Jobify/saved' }
      ]
    },
    {
      title: 'For Employers',
      links: [
        { label: 'Post a job', url: '/Jobify/post-job' },
        { label: 'Employer sign up', url: '/signup/employer' },
        { label: 'Manage applicants', url: '/Jobify/applicants' },
        { label: 'Pricing', url: '/pricing' }
      ]
    },
    {
      title: 'Company',
      links: [
        { label: 'About us', url: '/about' },
        { label: 'Contact', url: '/contact' },
        { label: 'Privacy policy', url: '/privacy' },
        { label: 'Terms of service', url: '/terms' }
      ]
    }
  ];
}