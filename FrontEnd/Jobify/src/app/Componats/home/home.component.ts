import { Component, inject, OnInit, signal } from '@angular/core';
import { JobService } from '../../Services/JobService/job.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { StatsSectionComponent } from '../../features/stats/stats-section/stats-section.component';
import { LatestJobsComponent } from '../../features/jobs/components/latest-jobs/latest-jobs.component';
import { TopHiringCompaniesComponent } from '../../features/Company/components/top-hiring-companies/top-hiring-companies.component';
import { JoinSectionComponent } from '../../features/JoinSection/join-section/join-section.component';
import { HowItWorksComponent } from '../../features/HowItWork/Component/how-it-works/how-it-works.component';
import { AuthserviceService } from '../../Services/AuthService/authservice.service';
import { IndustryService } from '../../features/Imdustry/Services/industry.service';
import { Industry, PopularIndustry } from '../../features/Imdustry/Model/industry.model';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, StatsSectionComponent, RouterLink,LatestJobsComponent, TopHiringCompaniesComponent, JoinSectionComponent,HowItWorksComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit  {
  private auth = inject(AuthserviceService);

 private industryService = inject(IndustryService);
popular = signal<PopularIndustry[]>([]);

  industries = signal<Industry[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
 private icons: Record<string, string> = {
    software: 'bi-code-slash',
    design: 'bi-palette',
    markting: 'bi-megaphone',  
    marketing: 'bi-megaphone',
    handmades: 'bi-scissors',
  };

  getIcon(slug: string): string {
    return this.icons[slug] ?? 'bi-briefcase'; 
  }
  ngOnInit() {
    this.industryService.getAll().subscribe({
      next: (data) => {
        this.industries.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.error.set('Failed to load industries');
        this.loading.set(false);
      },
    });


  this.industryService.getPopular(6).subscribe({
    next: (list) => this.popular.set(list),
    error: () => this.popular.set([])
  });
  }
 constructor(private api:JobService,private router: Router){

 }

onSearch(q: string, location: string, category: string) {
  this.router.navigate(['/jobs'], { queryParams: { q, location, category } });
}


  get postJobLink(): string {
    return this.auth.role() === 'Employer' ? '/Jobify/post-job' : '/signup/employer';
  }
}
