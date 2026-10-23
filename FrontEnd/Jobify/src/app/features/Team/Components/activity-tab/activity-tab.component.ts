import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivityItem, PagedResult } from '../../Models/team';
import { TeamService } from '../../Services/team.service';
import { PagerComponent } from '../pager/pager.component';

@Component({
  selector: 'app-activity-tab',
  standalone: true,
  imports: [CommonModule, PagerComponent],
  templateUrl: './activity-tab.component.html',
  styleUrl: './activity-tab.component.css'
})
export class ActivityTabComponent implements OnInit {
  filters = [
    { value: '', label: 'All' },
    { value: 'RoleChanged', label: 'Roles' },
    { value: 'PermissionsChanged', label: 'Permissions' },
    { value: 'AccessChanged', label: 'Access' },
    { value: 'MemberRemoved', label: 'Removed' },
    { value: 'MemberLeft', label: 'Left' },
    { value: 'MemberJoined', label: 'Joined' },
    { value: 'OwnershipTransferred', label: 'Ownership' }
  ];

  filter = { type: '', pageNumber: 1, pageSize: 15 };
  result: PagedResult<ActivityItem> | null = null;
  loading = false;
  error = '';

  constructor(private team: TeamService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.error = '';
    this.team.getActivity(this.filter).subscribe({
      next: r => { this.result = r; this.loading = false; },
      error: e => {
        this.loading = false;
        this.error = e.status === 403
          ? 'You do not have permission to view the activity log.'
          : e.error?.message ?? 'Could not load activity.';
      }
    });
  }

  setType(type: string): void {
    this.filter = { ...this.filter, type, pageNumber: 1 };
    this.load();
  }

  onPage(page: number): void {
    this.filter = { ...this.filter, pageNumber: page };
    this.load();
  }

  icon(type: string): string {
    switch (type) {
      case 'RoleChanged': return 'role';
      case 'PermissionsChanged': return 'perm';
      case 'AccessChanged': return 'access';
      case 'MemberRemoved': return 'remove';
      case 'MemberLeft': return 'leave';
      case 'MemberJoined': return 'join';
      case 'OwnershipTransferred': return 'owner';
      default: return 'other';
    }
  }

  timeAgo(iso: string): string {
    const value = iso.endsWith('Z') ? iso : iso + 'Z';
    const s = Math.floor((Date.now() - new Date(value).getTime()) / 1000);
    if (s < 60) return 'just now';
    const m = Math.floor(s / 60);
    if (m < 60) return `${m} min ago`;
    const h = Math.floor(m / 60);
    if (h < 24) return `${h} hour${h > 1 ? 's' : ''} ago`;
    const d = Math.floor(h / 24);
    if (d < 30) return `${d} day${d > 1 ? 's' : ''} ago`;
    return new Date(value).toLocaleDateString();
  }
}