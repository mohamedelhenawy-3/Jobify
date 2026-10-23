import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DEFAULT_TEAM_FILTER, MyTeamRole, PagedResult, TeamFilter, TeamMember } from '../../Models/team';
import { TeamService } from '../../Services/team.service';
import { MembersTableComponent } from '../members-table/members-table.component';
import { MembersFilterComponent } from '../members-filter/members-filter.component';
import { PagerComponent } from '../pager/pager.component';
import { AccessBlockedComponent } from '../../../../Shared/Componants/access-blocked/access-blocked.component';

@Component({
  selector: 'app-members-tab',
  standalone: true,
  imports: [CommonModule, MembersTableComponent, MembersFilterComponent, PagerComponent, AccessBlockedComponent],
  templateUrl: './members-tab.component.html',
  styleUrl: './members-tab.component.css'
})
export class MembersTabComponent implements OnInit {
  filter: TeamFilter = { ...DEFAULT_TEAM_FILTER };
  result: PagedResult<TeamMember> | null = null;
  me: MyTeamRole | null = null;

  meLoading = true;
  loading = false;
  error: string | null = null;

  constructor(private team: TeamService, private router: Router) {}

  get canUse(): boolean { return !!this.me && this.me.isActive && this.me.hasAccess; }

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
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

  load(): void {
    this.loading = true;
    this.error = null;

    this.team.getMembers(this.filter).subscribe({
      next: r => { this.result = r; this.loading = false; },
      error: e => {
        this.loading = false;
        const code = e.error?.code;
        if (e.status === 403 && (code === 'COMPANY_ACCESS_REVOKED' || code === 'ACCOUNT_SUSPENDED')) {
          this.reload();
          return;
        }
        this.error = e.error?.message ?? 'Could not load the team.';
      }
    });
  }

  onFilter(changes: Partial<TeamFilter>): void {
    this.filter = { ...this.filter, ...changes, pageNumber: 1 };
    this.load();
  }

  onPage(page: number): void {
    this.filter = { ...this.filter, pageNumber: page };
    this.load();
  }

  onLeft(): void {
    localStorage.removeItem('company');
    this.router.navigate(['/employer/no-company']);
  }
}