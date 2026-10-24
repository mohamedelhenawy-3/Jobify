import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DatePipe, NgClass } from '@angular/common';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

import { CompanyService } from '../../services/company.service';
import { CompanyMember, HiringSnapshot, MyCompany } from '../../models/company';
import { ActivityDto, ActivityFilter } from '../../../Activity/avrvity';
import { TeamService } from '../../../Team/Services/team.service';
import { IndustryService } from '../../../Imdustry/Services/industry.service';
import { Industry } from '../../../Imdustry/Model/industry.model';

type CompanyTab = 'overview' | 'team' | 'activity';

@Component({
  selector: 'app-my-company',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, RouterLink, DatePipe, NgClass],
  templateUrl: './my-company-component.component.html',
  styleUrl: './my-company-component.component.css'
})
export class MyCompanyComponent implements OnInit, OnDestroy {
  private service = inject(CompanyService);
  private teamService = inject(TeamService);
  private fb = inject(FormBuilder);
  private industryService = inject(IndustryService);

  company: MyCompany | null = null;
  loading = true;
  noCompany = false;
  editing = false;
  saving = false;
  message = '';
  error = '';

  activeTab: CompanyTab = 'overview';
industries: Industry[] = [];
  // team
  members: CompanyMember[] | null = null;
  membersTotal = 0;
  membersLoading = false;
  membersError = '';
  readonly previewSize = 5;

  // company activity
  activity: ActivityDto[] | null = null;
  activityLoading = false;
  activityError = '';
  activityTotal = 0;
  activityFilter: ActivityFilter = {
    search: '', category: '', from: '', to: '', actorEmployerId: null, pageNumber: 1, pageSize: 20
  };
  // keys must match ActivityCategories.Map on the server (personal ones are hidden by the API)
  readonly activityCategories = ['Members', 'Jobs', 'Applicants', 'Company'];

  private activityReq = 0;
  private search$ = new Subject<string>();
  private searchSub?: Subscription;

  readonly permissionLabels: Record<string, { label: string; icon: string }> = {
    PostJobs:             { label: 'Post jobs',             icon: 'fa-briefcase' },
    ManageApplicants:     { label: 'Manage applicants',     icon: 'fa-users' },
    ViewAnalytics:        { label: 'View analytics',        icon: 'fa-chart-line' },
    ManageCompanyProfile: { label: 'Edit company profile',  icon: 'fa-building' },
    ManageEmployerRoles:  { label: 'Manage team roles',     icon: 'fa-user-gear' },
    ApproveJoinRequests:  { label: 'Approve join requests', icon: 'fa-user-plus' }
  };

  get completeness() {
    const c = this.company;
    if (!c) return null;

    const items = [
      { label: 'Upload a company logo',                done: !!c.logo },
      { label: 'Write a description (50+ characters)', done: (c.description?.trim().length ?? 0) >= 50 },
      { label: 'Add your industry',                    done: !!c.industry?.trim() },
      { label: 'Choose a company size',                done: !!c.companySize?.trim() },
      { label: 'Add your location',                    done: !!c.location?.trim() },
      { label: 'Add the year you were founded',        done: !!c.founded?.trim() },
      { label: 'Add your website',                     done: !!c.companyWebsite?.trim() },
      { label: 'Add your industry', done: !!c.industryId },
    ];

    const doneCount = items.filter(i => i.done).length;
    return {
      percent: Math.round((doneCount / items.length) * 100),
      missing: items.filter(i => !i.done)
    };
  }

  logoFile: File | null = null;
  logoPreview: string | null = null;

  sizes = ['1-10', '11-50', '51-200', '201-500', '500+'];

form = this.fb.group({
  companyName: ['', [Validators.required, Validators.maxLength(200)]],
  companyWebsite: [''],
  description: ['', Validators.maxLength(5000)],
  location: [''],
  industryId: [null as number | null],
  companySize: [''],
  founded: ['']
});

  snapshot: HiringSnapshot | null = null;

  ngOnInit() {
    this.load();
    this.loadSnapshot();
  this.loadIndustries();

    this.searchSub = this.search$
      .pipe(debounceTime(350), distinctUntilChanged())
      .subscribe(s => {
        this.activityFilter.search = s;
        this.activityFilter.pageNumber = 1;
        this.loadActivity();
      });
  }

  ngOnDestroy() {
    this.searchSub?.unsubscribe();
  }

  can(permission: string): boolean {
    return this.company?.permissions.includes(permission) ?? false;
  }

  // Company-wide activity is only for people who can manage roles (same rule as the API)
  get canViewCompanyActivity(): boolean {
    return this.can('ManageEmployerRoles');
  }

  get tabs(): { value: CompanyTab; label: string; count?: number }[] {
    const list: { value: CompanyTab; label: string; count?: number }[] = [
      { value: 'overview', label: 'Overview' },
      { value: 'team', label: 'Team', count: this.members ? this.membersTotal : undefined }
    ];
    if (this.canViewCompanyActivity) list.push({ value: 'activity', label: 'Activity' });
    return list;
  }

  private loadSnapshot(): void {
    this.service.getHiringSnapshot().subscribe({
      next: (s) => (this.snapshot = s),
      error: () => (this.snapshot = null)    // the tiles simply don't render
    });
  }

  load() {
    this.loading = true;
    this.service.getMyCompany().subscribe({
      next: (c) => {
        this.company = c;
        this.loading = false;
      },
      error: (err) => {
        this.noCompany = err.status === 404;
        this.error = err.status === 404 ? '' : 'Could not load the company profile.';
        this.loading = false;
      }
    });
  }

  setTab(tab: CompanyTab) {
    this.activeTab = tab;
    if (tab === 'team' && this.members === null) this.loadMembers();
    if (tab === 'activity' && this.activity === null) this.loadActivity();
  }

  // ---------- Team ----------
  private loadMembers() {
    this.membersLoading = true;
    this.membersError = '';

    this.service.getMembersPreview(this.previewSize).subscribe({
      next: (r) => {
        this.members = r.items;
        this.membersTotal = r.total;
        this.membersLoading = false;
      },
      error: () => {
        this.membersError = 'Could not load team members.';
        this.membersLoading = false;
      }
    });
  }
private loadIndustries() {
  this.industryService.getAll(false).subscribe({
    next: (list) => (this.industries = list),
    error: () => (this.industries = [])
  });
}
  // ---------- Company activity ----------
  loadActivity() {
    const req = ++this.activityReq;   // ignore out-of-order responses
    this.activityLoading = true;
    this.activityError = '';

    this.teamService.getCompanyActivity(this.activityFilter).subscribe({
      next: (r) => {
        if (req !== this.activityReq) return;
        this.activity = r.items;
        this.activityTotal = r.totalCount;
        this.activityLoading = false;
      },
      error: (err) => {
        if (req !== this.activityReq) return;
        this.activityError = err.status === 401 || err.status === 403
          ? "You don't have permission to view company activity."
          : 'Could not load company activity.';
        this.activityLoading = false;
      }
    });
  }

  onActivitySearch(value: string) {
    this.search$.next(value);
  }

  applyActivityFilters() {
    this.activityFilter.pageNumber = 1;
    this.loadActivity();
  }

  clearActivityFilters() {
    this.activityFilter = {
      ...this.activityFilter, search: '', category: '', from: '', to: '', actorEmployerId: null, pageNumber: 1
    };
    this.loadActivity();
  }

  get hasActivityFilters(): boolean {
    const f = this.activityFilter;
    return !!(f.search || f.category || f.from || f.to || f.actorEmployerId);
  }

  get activityPages(): number {
    return Math.max(1, Math.ceil(this.activityTotal / this.activityFilter.pageSize));
  }

  goToActivityPage(page: number) {
    if (page < 1 || page > this.activityPages) return;
    this.activityFilter.pageNumber = page;
    this.loadActivity();
  }

  initials(name: string): string {
    return name.split(' ').filter(Boolean).slice(0, 2).map(p => p[0].toUpperCase()).join('') || '?';
  }

  // EF Core returns DateTime with Kind=Unspecified, so the JSON has no "Z" and the browser
  // reads the UTC value as local time. Treat values without a zone as UTC.
  toDate(iso: string): Date {
    return new Date(/([zZ]|[+-]\d\d:?\d\d)$/.test(iso) ? iso : iso + 'Z');
  }

  timeAgo(date: string): string {
    const seconds = Math.floor((Date.now() - this.toDate(date).getTime()) / 1000);
    const units: [number, string][] = [
      [31536000, 'y'], [2592000, 'mo'], [604800, 'w'], [86400, 'd'], [3600, 'h'], [60, 'm']
    ];
    for (const [secs, label] of units) {
      const v = Math.floor(seconds / secs);
      if (v >= 1) return `${v}${label} ago`;
    }
    return 'just now';
  }

  // ---------- Edit company ----------
  startEdit() {
    if (!this.company?.canEdit) return;

    this.form.patchValue({
      companyName: this.company.companyName,
      companyWebsite: this.company.companyWebsite ?? '',
      description: this.company.description ?? '',
      location: this.company.location ?? '',
      industryId: this.company.industryId ?? null,
      companySize: this.company.companySize ?? '',
      founded: this.company.founded ?? ''
    });

    this.logoFile = null;
    this.logoPreview = null;
    this.message = '';
    this.error = '';
    this.editing = true;
  }

  cancel() {
    this.editing = false;
    this.error = '';
  }

  onLogoSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/') || file.size > 2 * 1024 * 1024) {
      this.error = 'Logo must be an image under 2 MB.';
      return;
    }

    this.error = '';
    this.logoFile = file;
    this.logoPreview = URL.createObjectURL(file);
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = '';
const raw = this.form.getRawValue();
const payload = { ...raw, industryId: raw.industryId ? Number(raw.industryId) : null };

    this.service.updateMyCompany(payload, this.logoFile).subscribe({
      next: (c) => {
        this.company = c;
        this.saving = false;
        this.editing = false;
        this.message = 'Company profile updated successfully.';
      },
      error: (err) => {
        this.saving = false;
        if (err.status === 403) this.error = 'Only the company owner can edit this profile.';
        else if (err.status === 400) this.error = typeof err.error === 'string' ? err.error : 'Invalid data.';
        else this.error = 'Update failed. Please try again.';
      }
    });
  }
}