import { Component, inject, OnInit, signal } from '@angular/core';
import { CompanyService } from '../../services/company.service';
import { TopHiringCompany } from '../../models/company';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-top-hiring-companies',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './top-hiring-companies.component.html',
  styleUrl: './top-hiring-companies.component.css'
})
export class TopHiringCompaniesComponent 
implements OnInit {
  private companyService = inject(CompanyService);

  companies = signal<TopHiringCompany[]>([]);
  loading = signal(true);
  error = signal(false);

  readonly skeletons = [1, 2, 3, 4, 5, 6, 7, 8];

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(false);
    this.companyService.getTopHiring(8).subscribe({
      next: (data) => { this.companies.set(data); this.loading.set(false); },
      error: () => { this.error.set(true); this.loading.set(false); }
    });
  }
}
