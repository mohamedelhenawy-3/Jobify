import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin, Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs/operators';

import { OwnerProfileService } from '../Services/owner-profile.service';
import {
  EmployerJob,
  EmployerProfileDto,
  HiringPipelineDto,
  RecentApplicant,
  UpdateProfileForm
} from '../Models/OwnerProfile';
import { ActivityDto, ActivityFilter } from '../../Activity/avrvity';
import { JobService } from '../../jobs/Service/job.service';
import { JobApplicationService } from '../../../Services/JobApplication/jopapplicationservice.service';
import { TeamService } from '../../Team/Services/team.service';

type ProfileTab = 'overview' | 'jobs' | 'activity' | 'permissions';
type JobFilter = 'all' | 'active' | 'closed';

interface PipelineRow {
  label: string;
  count: number;
  percent: number;
  cls: string;
}

@Component({
  selector: 'app-owner-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './owner-profile.component.html',
  styleUrl: './owner-profile.component.css'
})
export class OwnerProfileComponent implements OnInit, OnDestroy {
  profile: EmployerProfileDto | null = null;
  loading = true;
  error: string | null = null;

  // edit profile
  showEditProfile = false;
  savingProfile = false;
  saveError: string | null = null;
  profileForm: UpdateProfileForm = {};
  selectedImage: File | null = null;
  imagePreview: string | null = null;

  activeTab: ProfileTab = 'overview';

  // jobs
  jobs: EmployerJob[] | null = null;
  jobsLoading = false;
  jobsError: string | null = null;
  jobFilter: JobFilter = 'all';

  // hiring
  pipeline: HiringPipelineDto | null = null;
  pipelineRows: PipelineRow[] = [];
  pipelineTotal = 0;
  recentApplicants: RecentApplicant[] = [];
  hiringLoading = false;

  // activity
  activity: ActivityDto[] | null = null;
  activityLoading = false;
  activityError: string | null = null;
  activityTotal = 0;
  activityFilter: ActivityFilter = {
    search: '', category: '', from: '', to: '', pageNumber: 1, pageSize: 15
  };
  // keys must match ActivityCategories.Map on the server
  readonly activityCategories = ['Members', 'Jobs', 'Applicants', 'Company'];

  private activityReq = 0;
  private search$ = new Subject<string>();
  private searchSub?: Subscription;

  readonly permissionCatalog = [
    { key: 'PostJobs',             label: 'Post jobs',               desc: 'Create and publish job listings.',            icon: 'fa-briefcase' },
    { key: 'ManageApplicants',     label: 'Manage applicants',       desc: 'Review applications and update their status.', icon: 'fa-users' },
    { key: 'ViewAnalytics',        label: 'View analytics',          desc: 'See hiring and job performance insights.',     icon: 'fa-chart-line' },
    { key: 'ManageCompanyProfile', label: 'Manage company profile',  desc: 'Edit the company name, logo and details.',    icon: 'fa-building' },
    { key: 'ManageEmployerRoles',  label: 'Manage team roles',       desc: 'Change roles and permissions of members.',     icon: 'fa-user-gear' },
    { key: 'ApproveJoinRequests',  label: 'Approve join requests',   desc: 'Accept or reject people joining the company.', icon: 'fa-user-plus' }
  ];

  private readonly maxImageSize = 2 * 1024 * 1024; // 2 MB

  constructor(
    private profileService: OwnerProfileService,
    private jobService: JobService,
    private applications: JobApplicationService,
    private teamservice: TeamService
  ) {}

  ngOnInit(): void {
    this.loadProfile();

    this.searchSub = this.search$
      .pipe(debounceTime(350), distinctUntilChanged())
      .subscribe(s => {
        this.activityFilter.search = s;
        this.activityFilter.pageNumber = 1;
        this.loadActivity();
      });
  }

  ngOnDestroy(): void {
    this.searchSub?.unsubscribe();
  }

  // ---------- Tabs ----------
  get tabs(): { value: ProfileTab; label: string; count?: number }[] {
    return [
      { value: 'overview', label: 'Overview' },
      // { value: 'jobs', label: this.isOwnerOrAdmin ? 'Company jobs' : 'My jobs', count: this.profile?.jobsPostedCount },
      { value: 'permissions', label: 'Permissions', count: this.profile?.permissions.length },
      { value: 'activity', label: 'Activity' }
    ];
  }

  setTab(tab: ProfileTab): void {
    this.activeTab = tab;
    if (tab === 'jobs' && this.jobs === null) this.loadJobs();
    if (tab === 'activity' && this.activity === null) this.loadActivity();
  }

  // ---------- Jobs ----------
  private loadJobs(): void {
    this.jobsLoading = true;
    this.jobsError = null;
    this.jobService.getMyJobs().subscribe({
      next: (j) => { this.jobs = j; this.jobsLoading = false; },
      error: () => { this.jobsError = 'Could not load jobs.'; this.jobsLoading = false; }
    });
  }

  get filteredJobs(): EmployerJob[] {
    const all = this.jobs ?? [];
    if (this.jobFilter === 'active') return all.filter(j => j.isActive);
    if (this.jobFilter === 'closed') return all.filter(j => !j.isActive);
    return all;
  }

  // ---------- Permissions ----------
  get grantedCount(): number {
    return this.permissionCatalog.filter(p => this.hasPermission(p.key)).length;
  }

  hasPermission(permission: string): boolean {
    return this.profile?.permissions.includes(permission) ?? false;
  }

  get isOwnerOrAdmin(): boolean {
    return this.profile?.role === 'Owner' || this.profile?.role === 'Admin';
  }

  // ---------- Profile ----------
  private loadProfile(): void {
    this.loading = true;
    this.profileService.getOwnerProfile().subscribe({
      next: (data) => {
        this.profile = data;
        console.log("my profiledata",data);
        this.loading = false;
        if (this.hasPermission('ManageApplicants') && !this.pipeline) this.loadHiring();
      },
      error: (err) => {
        this.error = err.status === 403 ? 'No access.' : 'Could not load profile.';
        this.loading = false;
      },
    });
  }

  // ---------- Hiring ----------
  private loadHiring(): void {
    this.hiringLoading = true;
    forkJoin({
      pipeline: this.applications.getPipeline(),
      recent: this.applications.getRecentApplicants(5)
    })
      .pipe(finalize(() => (this.hiringLoading = false)))
      .subscribe({
        next: ({ pipeline, recent }) => {
          this.pipeline = pipeline;
          this.recentApplicants = recent;
          this.buildPipelineRows(pipeline);
        },
        error: (err) => console.error('Failed to load hiring data', err)
      });
  }

  private buildPipelineRows(p: HiringPipelineDto): void {
    const defs = [
      { label: 'Applied',             count: p.applied,            cls: 'applied' },
      { label: 'Under review',        count: p.underReview,        cls: 'underreview' },
      { label: 'Shortlisted',         count: p.shortlisted,        cls: 'shortlisted' },
      { label: 'Interview scheduled', count: p.interviewScheduled, cls: 'interviewscheduled' },
      { label: 'Interviewed',         count: p.interviewed,        cls: 'interviewed' },
      { label: 'Offered',             count: p.offered,            cls: 'offered' },
      { label: 'Hired',               count: p.hired,              cls: 'hired' },
      { label: 'Rejected',            count: p.rejected,           cls: 'rejected' },
      { label: 'Withdrawn',           count: p.withdrawn,          cls: 'withdrawn' }
    ];

    this.pipelineTotal = defs.reduce((sum, d) => sum + d.count, 0);
    this.pipelineRows = defs.map(d => ({
      ...d,
      percent: this.pipelineTotal ? Math.round((d.count / this.pipelineTotal) * 100) : 0
    }));
  }

  // ---------- Activity ----------
  loadActivity(): void {
    const req = ++this.activityReq; // ignore out-of-order responses
    this.activityLoading = true;
    this.activityError = null;

    this.teamservice.getMyActivity(this.activityFilter).subscribe({
      next: (r) => {
        if (req !== this.activityReq) return;
        this.activity = r.items;
        this.activityTotal = r.totalCount;
        this.activityLoading = false;
      },
      error: (err) => {
        if (req !== this.activityReq) return;
        this.activityError = err.status === 401 || err.status === 403
          ? 'You are not in a company.'
          : 'Could not load activity.';
        this.activityLoading = false;
      }
    });
  }

  onActivitySearch(value: string): void {
    this.search$.next(value);
  }

  applyActivityFilters(): void {
    this.activityFilter.pageNumber = 1;
    this.loadActivity();
  }

  clearActivityFilters(): void {
    this.activityFilter = { ...this.activityFilter, search: '', category: '', from: '', to: '', pageNumber: 1 };
    this.loadActivity();
  }

  get hasActivityFilters(): boolean {
    const f = this.activityFilter;
    return !!(f.search || f.category || f.from || f.to);
  }

  get activityPages(): number {
    return Math.max(1, Math.ceil(this.activityTotal / this.activityFilter.pageSize));
  }

  goToActivityPage(page: number): void {
    if (page < 1 || page > this.activityPages) return;
    this.activityFilter.pageNumber = page;
    this.loadActivity();
  }

  initials(name: string): string {
    return name.split(' ').filter(Boolean).slice(0, 2).map(p => p[0].toUpperCase()).join('') || '?';
  }

  // ---------- Dates ----------
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

  // ---------- Edit profile ----------
  openEditProfile(): void {
    const p = this.profile;
    if (!p) return;

    this.profileForm = {
      fullName: p.fullName,
      phoneNumber: p.phone ?? '',
      location: p.location ?? '',
      position: p.position ?? p.headline ?? '',
      bio: p.bio ?? '',
      linkedIn: p.linkedIn ?? '',            // NEW

    };
    this.selectedImage = null;
    this.imagePreview = null;
    this.saveError = null;
    this.showEditProfile = true;
  }
    linkedInHref(url: string | null | undefined): string | null {
    const u = url?.trim();
    if (!u) return null;
    return /^https?:\/\//i.test(u) ? u : `https://${u}`;
  }

  closeEditProfile(): void {
    if (this.imagePreview) URL.revokeObjectURL(this.imagePreview);
    this.imagePreview = null;
    this.selectedImage = null;
    this.saveError = null;
    this.showEditProfile = false;
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      this.saveError = 'Please choose an image file.';
      input.value = '';
      return;
    }
    if (file.size > this.maxImageSize) {
      this.saveError = 'Image must be smaller than 2 MB.';
      input.value = '';
      return;
    }

    this.saveError = null;
    if (this.imagePreview) URL.revokeObjectURL(this.imagePreview);
    this.selectedImage = file;
    this.imagePreview = URL.createObjectURL(file);
  }

  submitProfile(): void {
    if (this.savingProfile) return;
    if (!this.profileForm.fullName?.trim()) {
      this.saveError = 'Full name is required.';
      return;
    }

    this.savingProfile = true;
    this.saveError = null;

    const li = this.profileForm.linkedIn?.trim();
if (li && !/linkedin\.com\//i.test(li)) {
  this.saveError = 'Enter a valid LinkedIn profile URL.';
  return;
}
    this.profileService.updateProfile(this.profileForm, this.selectedImage).subscribe({
      next: () => {
        this.savingProfile = false;
        this.closeEditProfile();
        this.loadProfile();
      },
      error: (err) => {
        this.savingProfile = false;
        this.saveError = err?.error?.message ?? 'Could not save your changes. Please try again.';
      },
    });
  }
}