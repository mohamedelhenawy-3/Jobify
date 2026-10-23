export interface EmployerProfile {
  userId: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
  position?: string;
  bio?: string;
  avatarUrl?: string;

  companyId: number;
  companyName: string;
  companyLogoUrl?: string;
  verificationStatus: string;
  isCompanyOwner: boolean;

  totalJobsPosted: number;
  activeJobsPosted: number;
  activeTeamMembersCount: number;
  pendingJoinRequestsCount: number;
}