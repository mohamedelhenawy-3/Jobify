import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CompanyService } from '../../services/company.service';
import { CompanyMember, HiringSnapshot, MyCompany } from '../../models/company';
import { RouterLink } from '@angular/router';
import { DatePipe, NgClass } from '@angular/common';




type CompanyTab = 'overview' | 'team';

@Component({
  selector: 'app-my-company',
  standalone: true,
  imports: [ReactiveFormsModule,ReactiveFormsModule, RouterLink, DatePipe, NgClass],
  templateUrl: './my-company-component.component.html',
  styleUrl: './my-company-component.component.css'
})
export class MyCompanyComponent implements OnInit {
  private service = inject(CompanyService);
  private fb = inject(FormBuilder);

  company: MyCompany | null = null;
  loading = true;
  noCompany = false;
  editing = false;
  saving = false;
  message = '';
  error = '';

activeTab: CompanyTab = 'overview';

members: CompanyMember[] | null = null;
membersTotal = 0;
membersLoading = false;
membersError = '';
readonly previewSize = 5;

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
    { label: 'Add your website',                     done: !!c.companyWebsite?.trim() }
  ];

  const doneCount = items.filter(i => i.done).length;
  return {
    percent: Math.round((doneCount / items.length) * 100),
    missing: items.filter(i => !i.done)
  };
}

// get verificationStep(): number {
//   return this.company?.verificationStatus === 'Approved' ? 3 : 2;
// }


  logoFile: File | null = null;
  logoPreview: string | null = null;

  sizes = ['1-10', '11-50', '51-200', '201-500', '500+'];

  form = this.fb.group({
    companyName: ['', [Validators.required, Validators.maxLength(200)]],
    companyWebsite: [''],
    description: ['', Validators.maxLength(5000)],
    location: [''],
    industry: [''],
    companySize: [''],
    founded: ['']
  });

  ngOnInit() {
    this.load();
    this.loadSnapshot();     

  }
can(permission: string): boolean {
  return this.company?.permissions.includes(permission) ?? false;
}
  get tabs(): { value: CompanyTab; label: string; count?: number }[] {
    return [
      { value: 'overview', label: 'Overview' },
      { value: 'team', label: 'Team', count: this.members ? this.membersTotal : undefined }
    ];
  }


snapshot: HiringSnapshot | null = null;

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
  }

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
  startEdit() {
    if (!this.company?.canEdit) return;

    this.form.patchValue({
      companyName: this.company.companyName,
      companyWebsite: this.company.companyWebsite ?? '',
      description: this.company.description ?? '',
      location: this.company.location ?? '',
      industry: this.company.industry ?? '',
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

    this.service.updateMyCompany(this.form.getRawValue(), this.logoFile).subscribe({
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