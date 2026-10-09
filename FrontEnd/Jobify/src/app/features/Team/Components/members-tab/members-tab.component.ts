import { Component } from '@angular/core';
import { DEFAULT_TEAM_FILTER, MyTeamRole, PagedResult, TeamFilter, TeamMember } from '../../Models/team';
import { TeamService } from '../../Services/team.service';
import { CommonModule } from '@angular/common';
import { MembersTableComponent } from '../members-table/members-table.component';
import { MembersFilterComponent } from '../members-filter/members-filter.component';
import { PagerComponent } from '../pager/pager.component';

@Component({
  selector: 'app-members-tab',
  standalone: true,
  imports: [CommonModule, MembersTableComponent, MembersFilterComponent, PagerComponent],
  templateUrl: './members-tab.component.html',
  styleUrl: './members-tab.component.css'
})
export class MembersTabComponent {
  filter: TeamFilter = { ...DEFAULT_TEAM_FILTER };
  result: PagedResult<TeamMember> | null = null;
  loading = false;
  error: string | null = null;
me: MyTeamRole | null = null;

   constructor(private team: TeamService) {}

  ngOnInit(): void {
    this.team.me().subscribe({ next: r => this.me = r, error: () => this.me = null });
    this.load();
  }
     load(): void {
    this.loading = true;
    this.error = null;

    this.team.getMembers(this.filter).subscribe({
      next: r => {
        this.result = r;
        this.loading = false;
        console.log('members', r);
      },
      error: e => {
        this.error = e.error?.message ?? 'Could not load the team.';
        this.loading = false;
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
  
}
