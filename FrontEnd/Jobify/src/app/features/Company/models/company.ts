
export interface MyCompany {
  id: number;
  logo?: string;
  companyName: string;
  companyWebsite?: string;
  description?: string;
  location?: string;
  permissions: string[];
  industry?: string;
  companySize?: string;
  founded?: string;
  isVerified: boolean;
  myRole: string;
  canEdit: boolean;
}
export interface HiringSnapshot {
  openJobs: number;
  newApplicantsThisWeek: number;
  interviewsScheduled: number;
  hiresThisMonth: number;
}
export type UpdateCompany = Omit<MyCompany,
  'id' | 'isVerified' | 'verificationStatus' | 'myRole' | 'canEdit'>;



export interface CompanyMember {
  logo:string
  employerId: number;
  fullName: string;
  email?: string | null;
  role: string;
  permissions?: string[];
  isActive: boolean;
  joinedCompanyAt?: string | null;
}

export interface CompanyMembersPreview {
  total: number;
  items: CompanyMember[];
}