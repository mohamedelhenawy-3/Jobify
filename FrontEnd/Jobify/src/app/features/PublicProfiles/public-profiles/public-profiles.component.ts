import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PublicEmployerProfile, PublicJobSeekerProfile, PublicProfile } from '../Models/profiles';
import { PublicProfileService } from '../Services/public-profile.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-public-profiles',
  standalone: true,
  imports: [RouterLink,CommonModule],
  templateUrl: './public-profiles.component.html',
  styleUrl: './public-profiles.component.css'
})
export class PublicProfilesComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private service = inject(PublicProfileService);
 
  profile: PublicProfile | null = null;
  loading = true;
  notFound = false;
  copied = false;
 
  companyPageReady = false;
 
  get employer(): PublicEmployerProfile | null {
    return this.profile?.type === 'employer' ? (this.profile as PublicEmployerProfile) : null;
  }
  get seeker(): PublicJobSeekerProfile | null {
    return this.profile?.type === 'jobseeker' ? (this.profile as PublicJobSeekerProfile) : null;
  }
 
  ngOnInit() {
    // runs again when the id in the URL changes
    this.route.paramMap.subscribe(params => {
      const id = params.get('id') ?? '';
 
      this.profile = null;
      this.loading = true;
      this.notFound = false;
 
      if (!id) { this.loading = false; this.notFound = true; return; }
 
      this.service.getProfile(id).subscribe({
        next: p => { this.profile = this.normalize(p); this.loading = false; },
        error: () => { this.loading = false; this.notFound = true; }
      });
    });
  }
 

  private normalize(p: PublicProfile): PublicProfile {
    const t = String(p.type ?? '').toLowerCase();
    return { ...p, type: t === 'employer' || t === 'enployer' ? 'employer' : 'jobseeker' } as PublicProfile;
  }
 
  get headline(): string {
    if (this.employer) {
      const e = this.employer;
      return [e.position, e.companyName ? `at ${e.companyName}` : null].filter(Boolean).join(' ');
    }
    return this.seeker?.headline ?? '';
  }
 
 get isPrivate(): boolean {
  return !!this.profile?.isPrivate;
}
get links(): { label: string; url: string }[] {
  if (this.isPrivate) return [];
  const out: { label: string; url: string }[] = [];
  const add = (label: string, url?: string | null) => { if (url) out.push({ label, url }); };
  if (this.employer) add('LinkedIn', this.employer.linkedInUrl);
  if (this.seeker) { add('GitHub', this.seeker.githubUrl); add('Website', this.seeker.websiteUrl); }
  return out;
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
 
