import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MyTeamRole } from '../../../features/Team/Models/team';
import { TeamService } from '../../../features/Team/Services/team.service';
// adjust to your real path

@Component({
  selector: 'app-access-blocked',
  standalone: true,
  imports: [CommonModule],
  template: `
    <!-- removed by the company owner/admin -->
    <div class="blocked-card" *ngIf="me && me.isActive && !me.hasAccess" role="alert">
      <div class="icon-wrap">
        <svg viewBox="0 0 24 24" width="32" height="32" fill="none" stroke="currentColor"
             stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <rect x="3" y="11" width="18" height="11" rx="2"/>
          <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
        </svg>
      </div>
      <h3>Company access restricted</h3>
      <p>
        Your access to this company has been restricted by an administrator.
        You can no longer view team members, jobs, applicants or other company data.
      </p>
      <p class="hint">
        Please contact your company owner to have your access restored,
        or leave the company to join a different one.
      </p>
      <button type="button" class="leave-btn" (click)="leaveCompany()" [disabled]="leaving">
        {{ leaving ? 'Leaving…' : 'Leave company' }}
      </button>
      <p class="error" *ngIf="leaveError">{{ leaveError }}</p>
    </div>

    <!-- suspended by the platform admin -->
    <div class="blocked-card" *ngIf="me && !me.isActive" role="alert">
      <div class="icon-wrap">
        <svg viewBox="0 0 24 24" width="32" height="32" fill="none" stroke="currentColor"
             stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <circle cx="12" cy="12" r="10"/>
          <line x1="4.9" y1="4.9" x2="19.1" y2="19.1"/>
        </svg>
      </div>
      <h3>Account suspended</h3>
      <p>
        Your account is currently suspended, so you can't use company features.
      </p>
      <p class="hint">
        If you believe this is a mistake, please contact our support team for help.
      </p>
    </div>
  `,
  styles: [`
    .blocked-card {
      background: #fef2f2;
      border: 1px solid #fecaca;
      color: #991b1b;
      border-radius: 12px;
      padding: 28px 24px;
      max-width: 520px;
      margin: 40px auto;
      text-align: center;
    }
    .icon-wrap {
      width: 64px; height: 64px; margin: 0 auto 12px;
      display: flex; align-items: center; justify-content: center;
      border-radius: 50%; background: #fee2e2; color: #b91c1c;
    }
    h3 { margin: 0 0 8px; font-size: 20px; }
    p { margin: 0 0 8px; line-height: 1.5; }
    .hint { font-size: 14px; color: #7f1d1d; opacity: .85; }
    .leave-btn {
      margin-top: 12px; border: 1px solid #fecaca; background: #fff; color: #b91c1c;
      border-radius: 8px; padding: 9px 20px; cursor: pointer; font-weight: 600;
    }
    .leave-btn:hover:not(:disabled) { background: #fee2e2; }
    .leave-btn:disabled { opacity: .6; cursor: not-allowed; }
    .error { color: #c0392b; font-size: 13px; margin-top: 10px; }
  `]
})
export class AccessBlockedComponent {
  @Input() me: MyTeamRole | null = null;
  @Output() left = new EventEmitter<void>();

  leaving = false;
  leaveError = '';

  constructor(private team: TeamService) {}

  leaveCompany(): void {
    if (!confirm('Are you sure you want to leave this company?')) return;
    this.leaving = true;
    this.leaveError = '';
    this.team.leave().subscribe({
      next: () => this.left.emit(),
      error: e => {
        this.leaving = false;
        this.leaveError = e.error?.message ?? 'Something went wrong. Please try again.';
      }
    });
  }
}