import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { ALL_PERMISSIONS, MANAGE_ROLES, MyTeamRole, ROLE_NAMES, roleId, RoleName, roleName, roleRank, TeamMember } from '../../Models/team';
import { TeamService } from '../../Services/team.service';

type Mode = 'role' | 'permissions' | 'remove' | 'transfer' | 'leave';

@Component({
  selector: 'app-members-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './members-table.component.html',
  styleUrl: './members-table.component.css'
})
export class MembersTableComponent {
  @Input() members: TeamMember[] = [];
  @Input() me: MyTeamRole | null = null;
  @Output() changed = new EventEmitter<void>();
  @Output() left = new EventEmitter<void>();

  roleId = roleId;
  allPermissions = ALL_PERMISSIONS;

  menuOpenId: number | null = null;
  activeId: number | null = null;
  mode: Mode | null = null;
  selectedRole: RoleName = 'Recruiter';
  draft = new Set<string>();
  reassignId: number | null = null;
  saving = false;
  error = '';

  constructor(private api: TeamService) {}

  roleName(role: number | string): string {
    return ROLE_NAMES[roleId(role)] ?? '';
  }

  get target(): TeamMember | undefined {
    return this.members.find(m => m.id === this.activeId);
  }

  get myRole(): RoleName | null {
    return this.me ? roleName(this.me.role) : null;
  }

  get assignableRoles(): RoleName[] {
    if (!this.myRole) return [];
    return (['Admin', 'Recruiter', 'Employer'] as RoleName[])
      .filter(r => roleRank(r) < roleRank(this.myRole!));
  }

  get reassignOptions(): TeamMember[] {
    return this.members.filter(x => x.isActive && x.hasAccessed && !x.isMe && x.id !== this.target?.id);
  }

  @HostListener('document:keydown.escape')
  onEsc() {
    if (!this.saving) this.close();
  }

  @HostListener('document:click')
  closeMenu() {
    this.menuOpenId = null;
  }

  initials(name: string): string {
    return name.split(' ').map(p => p[0]).slice(0, 2).join('').toUpperCase();
  }

  canManage(m: TeamMember): boolean {
    if (!this.me || !this.myRole) return false;
    return this.me.permissions.includes(MANAGE_ROLES)
      && !m.isMe
      && roleName(m.role) !== 'Owner'
      && roleRank(this.myRole) > roleRank(m.role);
  }

  canLeave(m: TeamMember): boolean {
    return m.isMe && this.myRole !== null && this.myRole !== 'Owner';
  }

  actionsFor(m: TeamMember) {
    const list: { key: string; label: string; danger?: boolean }[] = [
      { key: 'role', label: 'Change role' },
      { key: 'permissions', label: 'Edit permissions' },
      { key: 'active', label: m.hasAccessed ? 'Deny access' : 'Allow access' },
    ];
    if (this.myRole === 'Owner' && m.isActive && m.hasAccessed) {
      list.push({ key: 'transfer', label: 'Transfer ownership' });
    }
    list.push({ key: 'remove', label: 'Remove from company', danger: true });
    return list;
  }

  canToggle(perm: string): boolean {
    if (this.draft.has(perm)) return true;
    return this.myRole === 'Owner' || !!this.me?.permissions.includes(perm);
  }

  onReassign(ev: Event) {
    const v = (ev.target as HTMLSelectElement).value;
    this.reassignId = v ? +v : null;
  }

  toggleMenu(ev: Event, id: number) {
    ev.stopPropagation();
    this.menuOpenId = this.menuOpenId === id ? null : id;
  }

  choose(m: TeamMember, key: string) {
    this.menuOpenId = null;
    this.error = '';
    this.reassignId = null;

    if (key === 'active') {
      this.activeId = m.id;
      this.mode = null;
      this.run(this.api.setActive(m.id, !m.hasAccessed));
      return;
    }
    if (key === 'role') this.selectedRole = roleName(m.role);
    if (key === 'permissions') this.draft = new Set(m.permissions);

    this.activeId = m.id;
    this.mode = key as Mode;
  }

  startLeave(m: TeamMember) {
    this.error = '';
    this.reassignId = null;
    this.activeId = m.id;
    this.mode = 'leave';
  }

  close() {
    this.activeId = null;
    this.mode = null;
    this.error = '';
    this.reassignId = null;
  }

  togglePerm(key: string) {
    if (this.draft.has(key)) this.draft.delete(key);
    else this.draft.add(key);
  }

  private errorText(e: any): string {
    return typeof e.error === 'string' ? e.error
      : e.error?.message ?? e.error?.title ?? 'Something went wrong.';
  }

  private run(call: Observable<unknown>) {
    this.saving = true;
    this.error = '';
    call.subscribe({
      next: () => {
        this.saving = false;
        this.close();
        this.changed.emit();
      },
      error: e => {
        this.saving = false;
        this.activeId = null;
        this.error = this.errorText(e);
        if (this.mode) return;
        this.changed.emit();
      }
    });
  }

  saveRole(m: TeamMember) {
    this.run(this.api.changeRole(m.id, this.selectedRole));
  }

  savePermissions(m: TeamMember) {
    this.run(this.api.updatePermissions(m.id, [...this.draft]));
  }

  doRemove(m: TeamMember) {
    this.run(this.api.remove(m.id, this.reassignId));
  }

  doTransfer(m: TeamMember) {
    this.run(this.api.transferOwnership(m.id));
  }

  doLeave() {
    this.saving = true;
    this.error = '';
    this.api.leave(this.reassignId).subscribe({
      next: () => {
        this.saving = false;
        this.close();
        this.left.emit();
      },
      error: e => {
        this.saving = false;
        this.error = this.errorText(e);
      }
    });
  }
}