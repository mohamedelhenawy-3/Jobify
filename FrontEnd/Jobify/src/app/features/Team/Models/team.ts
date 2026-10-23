export const ROLE_NAMES: Record<number, string> = {
  1: 'Owner', 2: 'Employer', 3: 'Admin', 4: 'Recruiter'
};

const ROLE_IDS: Record<string, number> = {
  Owner: 1, Employer: 2, Admin: 3, Recruiter: 4
};

export function roleId(role: number | string): number {
  return typeof role === 'number' ? role : (ROLE_IDS[role] ?? 0);
}

export type RoleName = 'Owner' | 'Admin' | 'Recruiter' | 'Employer';

// NEW: always returns the role name, from a number or a string
export function roleName(role: number | string): RoleName {
  return (typeof role === 'number' ? ROLE_NAMES[role] : role) as RoleName;
}

export interface TeamMember {
  id: number;
  fullName: string;
  email: string;
  role: number | string;     // changed
  isActive: boolean;
  permissions: string[];
  jobsPosted: number;
  joinedAt: string | null;
  isMe: boolean;
  hasAccessed:boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface TeamFilter {
  search?: string;
  role?: number | string | null;                       // NEW: string added, the dropdown sends "Recruiter"
  isActive?: boolean | null;
  sortBy?: 'name' | 'joined' | 'jobs' | 'role';        // NEW: 'role' added
  pageNumber: number;
  pageSize: number;
}

export const DEFAULT_TEAM_FILTER: TeamFilter = {
  pageNumber: 1,
  pageSize: 10,
  sortBy: 'name'
};

export const ROLE_RANK: Record<RoleName, number> = {
  Owner: 3, Admin: 2, Recruiter: 1, Employer: 1     // same ranks as the backend
};

export function roleRank(role: number | string): number {
  return ROLE_RANK[roleName(role)] ?? 0;
}

export const MANAGE_ROLES = 'ManageEmployerRoles';

export const ALL_PERMISSIONS = [
  { key: 'PostJobs',             label: 'Post jobs' },
  { key: 'ManageApplicants',     label: 'Manage applicants' },
  { key: 'ViewAnalytics',        label: 'View analytics' },
  { key: 'ManageCompanyProfile', label: 'Manage company profile' },
  { key: 'ManageEmployerRoles',  label: 'Manage team roles' },
  { key: 'ApproveJoinRequests',  label: 'Approve join requests' }
];

export interface ActionResult { message: string; }

export interface MyTeamRole {
  role: number | string;
  permissions: string[];
  isActive: boolean;    
  hasAccess   :boolean;
}


export interface ActivityItem {
  id: number;
  type: string;
  message: string;
  actorName: string;
  targetName?: string | null;
  createdAt: string;
}