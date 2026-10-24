import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

interface Step {
  title: string;
  text: string;
}

@Component({
  selector: 'app-how-it-works',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './how-it-works.component.html',
  styleUrls: ['./how-it-works.component.css']
})
export class HowItWorksComponent {
  active: 'seekers' | 'employers' = 'seekers';

  seekerSteps: Step[] = [
    { title: 'Create account', text: 'Tell us about your skills and experience.' },
    { title: 'Search and apply', text: 'Filter thousands of jobs and apply with one click.' },
    { title: 'Get hired', text: 'Interview, receive offers, and start your new role.' }
  ];

  employerSteps: Step[] = [
    { title: 'Create company profile', text: 'Set up your company page and showcase your brand.' },
    { title: 'Post a job', text: 'Publish openings in minutes and reach qualified candidates.' },
    { title: 'Hire the best', text: 'Review applicants, interview, and make your offer.' }
  ];

  get steps(): Step[] {
    return this.active === 'seekers' ? this.seekerSteps : this.employerSteps;
  }
}