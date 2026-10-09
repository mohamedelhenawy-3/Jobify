import { Component } from '@angular/core';
import { EducationDto, ExperienceDto, ProfileDto } from '../../Models/seeker';
import { ProfileService } from '../../Service/profile.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-seeker-profile',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './seeker-profile.component.html',
  styleUrl: './seeker-profile.component.css'
})
export class SeekerProfileComponent {

 profile: ProfileDto | null = null;
  loading = true;
  busy = false;
  error = '';

  // form fields
  headline = '';
  location = '';
  phone = '';
  linkedInUrl = '';
  gitHubUrl = '';
  websiteUrl = '';
  isOpenToWork = false;
  about = '';
  skills: string[] = [];
  newSkill = '';


  showPrefs = false;

prefRole = '';
prefJobType = '';
prefSalary: number | null = null;
prefNotice = '';
prefLocations = '';

jobTypes = ['Full-time', 'Part-time', 'Contract', 'Internship', 'Freelance', 'Remote'];
noticePeriods = ['Immediately', '1 week', '2 weeks', '1 month', '2 months', '3 months'];
  constructor(private api: ProfileService) {}

  ngOnInit() {
    this.api.me().subscribe({
      next: p => { this.fill(p); 
        this.loading = false; 
      console.log('profile',p);
    },
      error: e => { this.error = this.getMsg(e); this.loading = false; }
    });
  }

  // copy server data into the page fields
  fill(p: ProfileDto) {
    this.profile = p;
    this.headline = p.headline ?? '';
    this.location = p.location ?? '';
    this.phone = p.phone ?? '';
    this.linkedInUrl = p.linkedInUrl ?? '';
    this.gitHubUrl = p.gitHubUrl ?? '';
    this.websiteUrl = p.websiteUrl ?? '';
    this.isOpenToWork = p.isOpenToWork;
    this.about = p.about ?? '';
    this.skills = [...p.skills];
    this.prefRole = p.desiredRole ?? '';
this.prefJobType = p.preferredJobType ?? '';
this.prefSalary = p.expectedSalary ?? null;
this.prefNotice = p.noticePeriod ?? '';
this.prefLocations = p.preferredLocations ?? '';
  }

  savePrefs() {
  if (this.prefSalary !== null && this.prefSalary < 0) {
    this.error = "Salary can't be negative."; return;
  }
  this.handle(this.api.savePreferences({
    desiredRole: this.prefRole,
    preferredJobType: this.prefJobType,
    expectedSalary: this.prefSalary,        // null when the field is empty
    noticePeriod: this.prefNotice,
    preferredLocations: this.prefLocations
  }), () => this.showPrefs = false);
}
  getMsg(e: any): string {
    return e?.error?.message ?? 'Something went wrong.';
  }

  get hasCv(): boolean {
    return !!this.profile && !this.profile.missing.includes('Upload your CV');
  }

 
private handle(obs: any, onDone?: () => void) {
  this.busy = true;
  this.error = '';
  obs.subscribe({
    next: (p: ProfileDto) => {
      this.fill(p);
      this.busy = false;
      this.showToast('Profile updated');
      if (onDone) onDone();
    },
    error: (e: any) => { this.error = this.getMsg(e); this.busy = false; }
  });
}


saveBasic() {
  this.handle(this.api.updateBasic({
    headline: this.headline, location: this.location, phone: this.phone,
    linkedInUrl: this.linkedInUrl, gitHubUrl: this.gitHubUrl,
    websiteUrl: this.websiteUrl, isOpenToWork: this.isOpenToWork
  }), () => this.showBasic = false);
}

saveAbout() { this.handle(this.api.updateAbout(this.about), () => this.showAbout = false); }

saveSkills() { this.handle(this.api.saveSkills(this.skills), () => this.showSkills = false); }
  onPhoto(event: any) {
    const file: File = event.target.files[0];
    event.target.value = '';
    if (!file) return;
    if (!file.type.startsWith('image/') || file.size > 2 * 1024 * 1024) {
      this.error = 'Photo must be an image under 2 MB.';
      return;
    }
    this.handle(this.api.uploadPhoto(file));
  }

  onCv(event: any) {
    const file: File = event.target.files[0];
    event.target.value = '';
    if (!file) return;
    if (file.type !== 'application/pdf' || file.size > 5 * 1024 * 1024) {
      this.error = 'CV must be a PDF under 5 MB.';
      return;
    }
    this.handle(this.api.uploadCv(file));
  }

  removeCv() { this.handle(this.api.removeCv()); }

  addSkill() {
    const s = this.newSkill.trim();
    this.newSkill = '';
    if (!s) return;
    if (this.skills.some(x => x.toLowerCase() === s.toLowerCase())) return;
    if (this.skills.length >= 30) { this.error = 'You can add up to 30 skills.'; return; }
    this.skills.push(s);
  }

  removeSkill(s: string) {
    this.skills = this.skills.filter(x => x !== s);
  }

// modal visibility (showExpForm and showEduForm already exist)
showBasic = false;
showAbout = false;
showSkills = false;

toastMsg = '';



get initials(): string {
  const parts = (this.profile?.fullName ?? '').trim().split(' ').filter(x => x);
  return parts.slice(0, 2).map(x => x[0].toUpperCase()).join('');
}

ringOffset(score: number): number {
  return 213.6 * (1 - score / 100);   // 213.6 = circle circumference (2 * PI * 34)
}

showToast(msg: string) {
  this.toastMsg = msg;
  setTimeout(() => this.toastMsg = '', 2500);
}

get anyModalOpen(): boolean {
  return this.showBasic || this.showAbout || this.showSkills || this.showExpForm
      || this.showEduForm || this.showPrefs;
}

closeAll() {
  this.showBasic = this.showAbout = this.showSkills = this.showPrefs = false;
  this.showExpForm = this.showEduForm = false;
  this.error = '';
  if (this.profile) this.fill(this.profile);
}
  barColor(score: number): string {
    if (score >= 80) return '#16a34a';
    if (score >= 50) return '#f59e0b';
    return '#ef4444';
  }


  // ---------- Experience ----------
showExpForm = false;
expId: number | null = null;
exp: any = this.emptyExp();

emptyExp() {
  return { title: '', company: '', location: '', startDate: '', endDate: '', isCurrent: false, description: '' };
}

openAddExp() {
  this.expId = null;
  this.exp = this.emptyExp();
  this.showExpForm = true;
  this.error = '';
}

openEditExp(e: any) {
  this.expId = e.id;
  this.exp = {
    title: e.title,
    company: e.company,
    location: e.location ?? '',
    startDate: e.startDate ? e.startDate.substring(0, 10) : '',   // "2024-01-15T00:00:00" -> "2024-01-15"
    endDate: e.endDate ? e.endDate.substring(0, 10) : '',
    isCurrent: e.isCurrent,
    description: e.description ?? ''
  };
  this.showExpForm = true;
  this.error = '';
}

cancelExp() { this.showExpForm = false; }

saveExp() {
  if (!this.exp.title.trim() || !this.exp.company.trim()) {
    this.error = 'Title and company are required.'; return;
  }
  if (!this.exp.startDate) { this.error = 'Start date is required.'; return; }
  if (!this.exp.isCurrent && this.exp.endDate && this.exp.endDate < this.exp.startDate) {
    this.error = "End date can't be before the start date."; return;
  }

  const body = {
    ...this.exp,
    endDate: this.exp.isCurrent || !this.exp.endDate ? null : this.exp.endDate   // "" would break .NET binding
  };
  this.handle(this.api.saveExperience(this.expId, body), () => this.showExpForm = false);
}

deleteExp(id: number) {
  if (!confirm('Delete this experience?')) return;
  this.handle(this.api.deleteExperience(id));
}

// ---------- Education ----------
showEduForm = false;
eduId: number | null = null;
edu: any = this.emptyEdu();

emptyEdu() {
  return { school: '', degree: '', fieldOfStudy: '', startDate: '', endDate: '', description: '' };
}

openAddEdu() {
  this.eduId = null;
  this.edu = this.emptyEdu();
  this.showEduForm = true;
  this.error = '';
}

openEditEdu(e: any) {
  this.eduId = e.id;
  this.edu = {
    school: e.school,
    degree: e.degree,
    fieldOfStudy: e.fieldOfStudy ?? '',
    startDate: e.startDate ? e.startDate.substring(0, 10) : '',
    endDate: e.endDate ? e.endDate.substring(0, 10) : '',
    description: e.description ?? ''
  };
  this.showEduForm = true;
  this.error = '';
}

cancelEdu() { this.showEduForm = false; }

saveEdu() {
  if (!this.edu.school.trim() || !this.edu.degree.trim()) {
    this.error = 'School and degree are required.'; return;
  }
  if (!this.edu.startDate) { this.error = 'Start date is required.'; return; }
  if (this.edu.endDate && this.edu.endDate < this.edu.startDate) {
    this.error = "End date can't be before the start date."; return;
  }

  const body = { ...this.edu, endDate: this.edu.endDate || null };
  this.handle(this.api.saveEducation(this.eduId, body), () => this.showEduForm = false);
}

deleteEdu(id: number) {
  if (!confirm('Delete this education?')) return;
  this.handle(this.api.deleteEducation(id));
}
}
