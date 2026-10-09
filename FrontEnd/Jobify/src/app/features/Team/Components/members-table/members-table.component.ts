import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { ALL_PERMISSIONS, MANAGE_ROLES, MyTeamRole, ROLE_NAMES, roleId, RoleName, roleName, roleRank, TeamMember } from '../../Models/team';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { TeamService } from '../../Services/team.service';


type Mode = 'role' | 'permissions' | 'remove' | 'transfer';
@Component({
  selector: 'app-members-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './members-table.component.html',
  styleUrl: './members-table.component.css'
})
export class MembersTableComponent {
  @Input() members: TeamMember[] = [];
  roleId = roleId;   // makes the function usable in the template

roleName(role: number | string): string {
  return ROLE_NAMES[roleId(role)] ?? '';
}

  constructor(private api: TeamService) {}

  @Input() me: MyTeamRole | null = null;
  @Output() changed = new EventEmitter<void>();   // parent reloads the list
  allPermissions = ALL_PERMISSIONS;
   get target(): TeamMember | undefined {
    return this.members.find(m => m.id === this.activeId);
  }

  @HostListener('document:keydown.escape')
  onEsc() { if (!this.saving) this.close(); }
  menuOpenId: number | null = null;   // which row's ⋯ menu is open
  activeId: number | null = null;     // which row's panel is open
  mode: Mode | null = null;
  selectedRole: RoleName = 'Recruiter';
  draft = new Set<string>();
  saving = false;
  error = '';
  initials(name: string): string {
    return name.split(' ').map(p => p[0]).slice(0, 2).join('').toUpperCase();
  }


  @HostListener('document:click')
  closeMenu() { this.menuOpenId = null; }

  // ---------- who can do what (mirrors the backend) ----------
  // inside the class, roleName(...) without "this." is the imported function from the model
  get myRole(): RoleName | null { return this.me ? roleName(this.me.role) : null; }
     canManage(m: TeamMember): boolean {
    if (!this.me || !this.myRole) return false;
    return this.me.permissions.includes(MANAGE_ROLES)
      && !m.isMe
      && roleName(m.role) !== 'Owner'
      && roleRank(this.myRole) > roleRank(m.role);
  
}

  actionsFor(m: TeamMember) {
    const list: { key: string; label: string; danger?: boolean }[] = [
      { key: 'role', label: 'Change role' },
      { key: 'permissions', label: 'Edit permissions' },
      { key: 'active', label: m.isActive ? 'Deactivate' : 'Activate' },
    ];
    if (this.myRole === 'Owner') list.push({ key: 'transfer', label: 'Transfer ownership' });
    list.push({ key: 'remove', label: 'Remove from company', danger: true });
    return list;
  }
  get assignableRoles(): RoleName[] {
    if (!this.myRole) return [];
    return (['Admin', 'Recruiter', 'Employer'] as RoleName[])
      .filter(r => roleRank(r) < roleRank(this.myRole!));
  }
 canToggle(perm: string): boolean {
    if (this.draft.has(perm)) return true;   // may always remove
    return this.myRole === 'Owner' || !!this.me?.permissions.includes(perm);
  }

  // ---------- menu ----------
  toggleMenu(ev: Event, id: number) {
    ev.stopPropagation();
    this.menuOpenId = this.menuOpenId === id ? null : id;
  }

  choose(m: TeamMember, key: string) {
    this.menuOpenId = null;
    this.error = '';

    if (key === 'active') {
      this.activeId = m.id;
      this.mode = null;
      this.run(this.api.setActive(m.id, !m.isActive));
      return;
    }
    if (key === 'role') this.selectedRole = roleName(m.role);
    if (key === 'permissions') this.draft = new Set(m.permissions);

    this.activeId = m.id;
    this.mode = key as Mode;
  }

  close() { this.activeId = null; this.mode = null; this.error = ''; }

  togglePerm(key: string) {
    this.draft.has(key) ? this.draft.delete(key) : this.draft.add(key);
  }

  // ---------- actions ----------
  private run(call: Observable<unknown>) {
    this.saving = true;
    this.error = '';
    call.subscribe({
      next: () => { this.saving = false; this.close(); this.changed.emit(); },
      error: e => {
        this.saving = false;
        this.error = typeof e.error === 'string' ? e.error
                   : e.error?.message ?? e.error?.title ?? 'Something went wrong.';
      }
    });
  }

  saveRole(m: TeamMember)        { this.run(this.api.changeRole(m.id, roleId(this.selectedRole))); }
  savePermissions(m: TeamMember) { this.run(this.api.updatePermissions(m.id, [...this.draft])); }
  doRemove(m: TeamMember)        { this.run(this.api.remove(m.id)); }
  doTransfer(m: TeamMember)      { this.run(this.api.transferOwnership(m.id)); }

}
