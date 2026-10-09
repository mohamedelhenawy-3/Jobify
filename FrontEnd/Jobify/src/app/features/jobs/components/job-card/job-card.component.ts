import { Component, Input } from '@angular/core';
import { EmployerJobCard } from '../../models/Job';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-job-card',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './job-card.component.html',
  styleUrl: './job-card.component.css'
})
export class JobCardComponent {
// Inside your job card component (where onEditJob is defined)

constructor(private router:Router){

}
  @Input({ required: true }) job!: EmployerJobCard;

  get salary(): string | null {
    const { minSalary: min, maxSalary: max, currency } = this.job;
    if (min && max) return `${min.toLocaleString()} - ${max.toLocaleString()} ${currency}`;
    if (min) return `From ${min.toLocaleString()} ${currency}`;
    if (max) return `Up to ${max.toLocaleString()} ${currency}`;
    return null;
  }
}
