import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { JoinRequestFilter, JoinRequestItem, JoinStatus } from '../../Models/join-request';
import { MyTeamRole, PagedResult } from '../../Models/team';
import { JoinRequestServiceService } from '../../Services/onboarding/join-request-service.service';
import { TeamService } from '../../Services/team.service';
import { PagerComponent } from '../pager/pager.component';
import { AccessBlockedComponent } from '../../../../Shared/Componants/access-blocked/access-blocked.component';

@Component({
  selector: 'app-join-requests-tab',
  standalone: true,
  imports: [PagerComponent, CommonModule, FormsModule, AccessBlockedComponent],
  templateUrl: './join-requests-tab.component.html',
  styleUrl: './join-requests-tab.component.css'
})
export class JoinRequestsTabComponent implements OnInit {

  statuses: JoinStatus[] = ['Pending', 'Approved', 'Rejected'];

  filter: JoinRequestFilter = { status: 'Pending', pageNumber: 1, pageSize: 5 };
  result: PagedResult<JoinRequestItem> | null = null;
  loading = false;
  error = '';

  // confirm modal
  target: JoinRequestItem | null = null;
  approving = true;
  saving = false;
  modalError = '';

  // access state
  me: MyTeamRole | null = null;
  meLoading = true;

  constructor(
    private api: JoinRequestServiceService,
    private team: TeamService,
    private router: Router
  ) {}

  get canUse(): boolean { return !!this.me && this.me.isActive && this.me.hasAccess; }

  ngOnInit() { this.loadMe(); }

  loadMe() {
    this.meLoading = true;
    this.team.me().subscribe({
      next: r => {
        this.me = r;
        this.meLoading = false;
        if (this.canUse) this.load();
        else this.result = null;
      },
      error: e => {
        this.meLoading = false;
        this.result = null;
        if (e.status === 403) {
          const suspended = e.error?.code === 'ACCOUNT_SUSPENDED';
          this.me = { role: 'Employer', permissions: [], isActive: !suspended, hasAccess: false } as MyTeamRole;
          return;
        }
        this.me = null;
        this.error = e.error?.message ?? 'Could not load your role.';
      }
    });
  }

  onLeft() {
    localStorage.removeItem('company');
    this.router.navigate(['/employer/no-company']);
  }

  load() {
    this.loading = true;
    this.error = '';
    this.api.list(this.filter).subscribe({
      next: (r) => { this.result = r; this.loading = false; },
      error: (e) => {
        this.loading = false;
        const code = e.error?.code;
        if (e.status === 403 && (code === 'COMPANY_ACCESS_REVOKED' || code === 'ACCOUNT_SUSPENDED')) {
          this.result = null;
          this.loadMe();   // blocked while the page was open: show the banner
          return;
        }
        this.error = typeof e.error === 'string' ? e.error : e.error?.message ?? 'Could not load requests.';
      }
    });
  }

  setStatus(s: JoinStatus) {
    this.filter = { ...this.filter, status: s, pageNumber: 1 };
    this.load();
  }

  onPage(page: number) {
    this.filter = { ...this.filter, pageNumber: page };
    this.load();
  }

  initials(name: string) {
    return name.split(' ').map(p => p[0]).slice(0, 2).join('').toUpperCase();
  }

  open(r: JoinRequestItem, approve: boolean) {
    this.target = r; this.approving = approve; this.modalError = '';
  }

  close() { if (!this.saving) this.target = null; }

  confirm() {
    if (!this.target) return;
    const t = this.target;
    this.saving = true; this.modalError = '';

    this.api.process(t.id, this.approving).subscribe({
      next: () => this.afterProcess(),
      error: e => {
        this.saving = false;
        if (e.status === 403) { this.target = null; this.loadMe(); return; }
        this.modalError = typeof e.error === 'string' ? e.error : e.error?.message ?? 'Something went wrong.';
      }
    });
  }

  private afterProcess() {
    if (this.result && this.result.items.length === 1 && this.filter.pageNumber > 1) {
      this.filter = { ...this.filter, pageNumber: this.filter.pageNumber - 1 };
    }
    this.saving = false;
    this.target = null;
    this.load();
  }
}