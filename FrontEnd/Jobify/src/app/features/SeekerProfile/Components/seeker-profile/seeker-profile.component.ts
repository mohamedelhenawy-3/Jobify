import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, switchMap, tap } from 'rxjs';
import { ProfileDto, ProjectDto } from '../../Models/seeker';
import { ProfileService } from '../../Service/profile.service';

@Component({
  selector: 'app-seeker-profile',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './seeker-profile.component.html',
  styleUrl: './seeker-profile.component.css'
})
export class SeekerProfileComponent implements OnInit, OnDestroy {

  profile: ProfileDto | null = null;
  loading = true;
  busy = false;
  error = '';
  toastMsg = '';

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

  showBasic = false;
  showAbout = false;
  showSkills = false;
  showPrefs = false;
  showExpForm = false;
  showEduForm = false;
  showLangForm = false;
  showCertForm = false;
  showProjForm = false;

  prefRole = '';
  prefJobType = '';
  prefSalary: number | null = null;
  prefNotice = '';
  prefLocations = '';

  jobTypes = ['Full-time', 'Part-time', 'Contract', 'Internship', 'Freelance', 'Remote'];
  noticePeriods = ['Immediately', '1 week', '2 weeks', '1 month', '2 months', '3 months'];
  languageLevels = ['Basic', 'Conversational', 'Fluent', 'Native'];

  expId: number | null = null;
  exp: any = this.emptyExp();

  eduId: number | null = null;
  edu: any = this.emptyEdu();

  langId: number | null = null;
  lang: any = this.emptyLang();

  certId: number | null = null;
  cert: any = this.emptyCert();

  // ---------- projects / gallery ----------
  readonly MAX_IMAGES = 10;
  readonly MAX_IMAGE_BYTES = 5 * 1024 * 1024;

  projId: number | null = null;
  proj: any = this.emptyProj();
  pending: { file: File; url: string }[] = [];   // images chosen but not uploaded yet

  viewId: number | null = null;                  // project open in the viewer
  activeImg = 0;
  fitFull = false;                               // false = image fills the frame, true = show whole image

  @ViewChild('gal') gal?: ElementRef<HTMLElement>;

  constructor(private api: ProfileService) {}

  ngOnInit() {
    this.api.me().subscribe({
      next: p => { this.fill(p); this.loading = false; },
      error: e => { this.error = this.getMsg(e); this.loading = false; }
    });
  }

  ngOnDestroy() {
    this.clearPending();
  }

  fill(p: ProfileDto) {
    this.profile = {
      ...p,
      skills: p.skills ?? [],
      experiences: p.experiences ?? [],
      educations: p.educations ?? [],
      languages: p.languages ?? [],
      certificates: p.certificates ?? [],
      projects: (p.projects ?? []).map(x => ({ ...x, images: x.images ?? [] })),
      missing: p.missing ?? []
    };
    this.headline = p.headline ?? '';
    this.location = p.location ?? '';
    this.phone = p.phone ?? '';
    this.linkedInUrl = p.linkedInUrl ?? '';
    this.gitHubUrl = p.gitHubUrl ?? '';
    this.websiteUrl = p.websiteUrl ?? '';
    this.isOpenToWork = p.isOpenToWork;
    this.about = p.about ?? '';
    this.skills = [...(p.skills ?? [])];
    this.prefRole = p.desiredRole ?? '';
    this.prefJobType = p.preferredJobType ?? '';
    this.prefSalary = p.expectedSalary ?? null;
    this.prefNotice = p.noticePeriod ?? '';
    this.prefLocations = p.preferredLocations ?? '';
  }

  getMsg(e: any): string {
    return typeof e?.error === 'string' ? e.error : e?.error?.message ?? 'Something went wrong.';
  }

  get hasCv(): boolean {
    return !!this.profile && !this.profile.missing.includes('Upload your CV');
  }

  get hasPrefs(): boolean {
    const p = this.profile;
    return !!p && !!(p.desiredRole || p.preferredJobType || p.expectedSalary || p.noticePeriod || p.preferredLocations);
  }

  get initials(): string {
    const parts = (this.profile?.fullName ?? '').trim().split(' ').filter(x => x);
    return parts.slice(0, 2).map(x => x[0].toUpperCase()).join('');
  }

  get anyModalOpen(): boolean {
    return this.showBasic || this.showAbout || this.showSkills || this.showPrefs
      || this.showExpForm || this.showEduForm || this.showLangForm
      || this.showCertForm || this.showProjForm || this.viewId !== null;
  }

  private handle(obs: any, onDone?: () => void) {
    this.busy = true;
    this.error = '';
    obs.subscribe({
      next: (p: any) => {
        if (p && Array.isArray(p.skills)) {
          this.fill(p);
          this.busy = false;
          this.showToast('Profile updated');
          if (onDone) onDone();
        } else {
          this.api.me().subscribe({
            next: full => {
              this.fill(full);
              this.busy = false;
              this.showToast('Profile updated');
              if (onDone) onDone();
            },
            error: (e: any) => { this.error = this.getMsg(e); this.busy = false; }
          });
        }
      },
      error: (e: any) => { this.error = this.getMsg(e); this.busy = false; }
    });
  }

  showToast(msg: string) {
    this.toastMsg = msg;
    setTimeout(() => this.toastMsg = '', 2500);
  }

  ringOffset(score: number): number {
    return 213.6 * (1 - score / 100);
  }

  barColor(score: number): string {
    if (score >= 80) return '#16a34a';
    if (score >= 50) return '#f59e0b';
    return '#ef4444';
  }

  closeAll() {
    this.showBasic = this.showAbout = this.showSkills = this.showPrefs = false;
    this.showExpForm = this.showEduForm = false;
    this.showLangForm = this.showCertForm = this.showProjForm = false;
    this.viewId = null;
    this.clearPending();
    this.error = '';
    if (this.profile) this.fill(this.profile);
  }

  saveBasic() {
    this.handle(this.api.updateBasic({
      headline: this.headline, location: this.location, phone: this.phone,
      linkedInUrl: this.linkedInUrl, gitHubUrl: this.gitHubUrl,
      websiteUrl: this.websiteUrl, isOpenToWork: this.isOpenToWork
    }), () => this.showBasic = false);
  }

  saveAbout() {
    this.handle(this.api.updateAbout(this.about), () => this.showAbout = false);
  }

  saveSkills() {
    this.handle(this.api.saveSkills(this.skills), () => this.showSkills = false);
  }

  savePrefs() {
    if (this.prefSalary !== null && this.prefSalary < 0) {
      this.error = "Salary can't be negative."; return;
    }
    this.handle(this.api.savePreferences({
      desiredRole: this.prefRole,
      preferredJobType: this.prefJobType,
      expectedSalary: this.prefSalary,
      noticePeriod: this.prefNotice,
      preferredLocations: this.prefLocations
    }), () => this.showPrefs = false);
  }

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

  removeCv() {
    this.handle(this.api.removeCv());
  }

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

  // ---------- experience ----------
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
      startDate: e.startDate ? e.startDate.substring(0, 10) : '',
      endDate: e.endDate ? e.endDate.substring(0, 10) : '',
      isCurrent: e.isCurrent,
      description: e.description ?? ''
    };
    this.showExpForm = true;
    this.error = '';
  }

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
      endDate: this.exp.isCurrent || !this.exp.endDate ? null : this.exp.endDate
    };
    this.handle(this.api.saveExperience(this.expId, body), () => this.showExpForm = false);
  }

  deleteExp(id: number) {
    if (!confirm('Delete this experience?')) return;
    this.handle(this.api.deleteExperience(id));
  }

  // ---------- education ----------
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

  // ---------- languages ----------
  emptyLang() {
    return { name: '', level: 'Fluent' };
  }

  openAddLang() {
    this.langId = null;
    this.lang = this.emptyLang();
    this.showLangForm = true;
    this.error = '';
  }

  openEditLang(l: any) {
    this.langId = l.id;
    this.lang = { name: l.name, level: l.level };
    this.showLangForm = true;
    this.error = '';
  }

  saveLang() {
    if (!this.lang.name.trim()) { this.error = 'Language is required.'; return; }
    this.handle(this.api.saveLanguage(this.langId, this.lang), () => this.showLangForm = false);
  }

  deleteLang(id: number) {
    if (!confirm('Delete this language?')) return;
    this.handle(this.api.deleteLanguage(id));
  }

  // ---------- certificates ----------
  emptyCert() {
    return { name: '', issuer: '', issueDate: '', expiryDate: '', credentialUrl: '' };
  }

  openAddCert() {
    this.certId = null;
    this.cert = this.emptyCert();
    this.showCertForm = true;
    this.error = '';
  }

  openEditCert(c: any) {
    this.certId = c.id;
    this.cert = {
      name: c.name,
      issuer: c.issuer,
      issueDate: c.issueDate ? c.issueDate.substring(0, 10) : '',
      expiryDate: c.expiryDate ? c.expiryDate.substring(0, 10) : '',
      credentialUrl: c.credentialUrl ?? ''
    };
    this.showCertForm = true;
    this.error = '';
  }

  saveCert() {
    if (!this.cert.name.trim() || !this.cert.issuer.trim()) {
      this.error = 'Name and issuer are required.'; return;
    }
    if (!this.cert.issueDate) { this.error = 'Issue date is required.'; return; }
    if (this.cert.expiryDate && this.cert.expiryDate < this.cert.issueDate) {
      this.error = "Expiry date can't be before the issue date."; return;
    }
    const body = {
      ...this.cert,
      expiryDate: this.cert.expiryDate || null,
      credentialUrl: this.cert.credentialUrl.trim() || null
    };
    this.handle(this.api.saveCertificate(this.certId, body), () => this.showCertForm = false);
  }

  deleteCert(id: number) {
    if (!confirm('Delete this certificate?')) return;
    this.handle(this.api.deleteCertificate(id));
  }

  // =============== PROJECTS / GALLERY ===============

  emptyProj() {
    return { title: '', description: '', url: '', technologies: '' };
  }

  get editingProject(): ProjectDto | null {
    return this.profile?.projects.find(x => x.id === this.projId) ?? null;
  }

  get viewProject(): ProjectDto | null {
    return this.profile?.projects.find(x => x.id === this.viewId) ?? null;
  }

  /** existing images + images waiting to be uploaded */
  get imageCount(): number {
    return (this.editingProject?.images.length ?? 0) + this.pending.length;
  }

  trackById(_: number, p: ProjectDto) { return p.id; }

  openAddProj() {
    this.projId = null;
    this.proj = this.emptyProj();
    this.clearPending();
    this.showProjForm = true;
    this.error = '';
  }

  openEditProj(p: ProjectDto) {
    this.viewId = null;
    this.projId = p.id ?? null;
    this.proj = {
      title: p.title,
      description: p.description ?? '',
      url: p.url ?? '',
      technologies: p.technologies ?? ''
    };
    this.clearPending();
    this.showProjForm = true;
    this.error = '';
  }

  onProjImages(event: any) {
    const files: File[] = Array.from(event.target.files ?? []);
    event.target.value = '';
    this.error = '';

    for (const f of files) {
      if (!f.type.startsWith('image/') || f.size > this.MAX_IMAGE_BYTES) {
        this.error = 'Each file must be an image up to 5 MB.';
        continue;
      }
      if (this.imageCount >= this.MAX_IMAGES) {
        this.error = `A project can have up to ${this.MAX_IMAGES} images.`;
        break;
      }
      this.pending.push({ file: f, url: URL.createObjectURL(f) });
    }
  }

  removePending(i: number) {
    URL.revokeObjectURL(this.pending[i].url);
    this.pending.splice(i, 1);
  }

  private clearPending() {
    this.pending.forEach(x => URL.revokeObjectURL(x.url));
    this.pending = [];
  }

  /** remove an already-uploaded image (takes effect immediately) */
  deleteImage(imageId: number) {
    if (this.projId === null) return;
    if (!confirm('Delete this image?')) return;
    this.handle(this.api.deleteProjectImage(this.projId, imageId));
  }

  setCover(imageId: number) {
    if (this.projId === null) return;
    this.handle(this.api.setProjectCover(this.projId, imageId));
  }

  private newestProjectId(p: ProfileDto): number | null {
    const ids = (p.projects ?? []).map(x => x.id ?? 0);
    return ids.length ? Math.max(...ids) : null;
  }

  saveProj() {
    if (!this.proj.title.trim()) { this.error = 'Title is required.'; return; }

    const body = {
      title: this.proj.title.trim(),
      description: this.proj.description.trim() || null,
      url: this.proj.url.trim() || null,
      technologies: this.proj.technologies.trim() || null
    };

    this.busy = true;
    this.error = '';

    // 1) save the project (JSON)  2) upload the new images (FormData)
    this.api.saveProject(this.projId, body).pipe(
      switchMap(res => Array.isArray((res as any)?.skills) ? of(res) : this.api.me()),
      tap(p => {
        this.fill(p);
        // remember the new id so a retry after a failed upload doesn't create a duplicate
        if (this.projId === null) this.projId = this.newestProjectId(p);
      }),
      switchMap(p => {
        if (!this.pending.length || this.projId === null) return of(p);
        return this.api.addProjectImages(this.projId, this.pending.map(x => x.file));
      })
    ).subscribe({
      next: p => {
        this.fill(p);
        this.busy = false;
        this.clearPending();
        this.showProjForm = false;
        this.showToast('Project saved');
      },
      error: e => { this.error = this.getMsg(e); this.busy = false; }
    });
  }

  deleteProj(id: number) {
    if (!confirm('Delete this project and its images?')) return;
    this.handle(this.api.deleteProject(id), () => this.viewId = null);
  }

  techList(t: string | null): string[] {
    return (t ?? '').split(',').map(x => x.trim()).filter(x => x);
  }

  // ----- gallery strip + viewer -----
  scrollGallery(dir: number) {
    const el = this.gal?.nativeElement;
    if (!el) return;
    el.scrollBy({ left: dir * el.clientWidth * 0.9, behavior: 'smooth' });
  }

  openView(p: ProjectDto) {
    this.viewId = p.id ?? null;
    this.fitFull = false;
    const i = p.images.findIndex(x => x.isCover);
    this.activeImg = i >= 0 ? i : 0;
  }

  stepImg(dir: number, p: ProjectDto) {
    const n = p.images.length;
    if (n < 2) return;
    this.activeImg = (this.activeImg + dir + n) % n;
  }

  // ---------- visibility ----------
  visBusy = false;

  toggleVisibility() {
    if (!this.profile || this.visBusy) return;
    const next = !this.profile.isPublic;
    this.visBusy = true;
    this.error = '';

    this.api.saveVisibility({ isPublic: next }).subscribe({
      next: () => {
        this.profile!.isPublic = next;
        this.visBusy = false;
        this.showToast(next ? 'Your profile is now public' : 'Your profile is now private');
      },
      error: e => {
        this.visBusy = false;
        this.error = this.getMsg(e);
      }
    });
  }
}