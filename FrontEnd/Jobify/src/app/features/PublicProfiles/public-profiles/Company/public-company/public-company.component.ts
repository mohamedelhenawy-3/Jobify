import { Component, inject } from '@angular/core';
import { PublicCompany } from '../../../Models/profiles';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PublicProfileService } from '../../../Services/public-profile.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-public-company',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './public-company.component.html',
  styleUrl: './public-company.component.css'
})
export class PublicCompanyComponent {
 private route = inject(ActivatedRoute);
  private service = inject(PublicProfileService);
 
  company: PublicCompany | null = null;
  loading = true;
  notFound = false;
  copied = false;
 
  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
 
      this.company = null;
      this.loading = true;
      this.notFound = false;
 
      if (!id) { this.loading = false; this.notFound = true; return; }
 
      this.service.getCompany(id).subscribe({
        next: c => { this.company = c; this.loading = false; },
        error: () => { this.loading = false; this.notFound = true; }
      });
    });
  }
 
  websiteLabel(url: string): string {
    return url.replace(/^https?:\/\//i, '').replace(/\/$/, '');
  }
 
  isSafeUrl(url?: string | null): boolean {
    return !!url && /^https?:\/\//i.test(url);
  }
 
  initials(name: string): string {
    return (name || '?').trim().split(/\s+/).slice(0, 2).map(w => w[0]).join('').toUpperCase();
  }
 
  async copyLink() {
    try { await navigator.clipboard.writeText(window.location.href); this.copied = true; }
    catch { this.copied = false; }
    setTimeout(() => (this.copied = false), 2000);
  }
}
