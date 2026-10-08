export interface EmployerProfileDto {
  fullName: string;
  headline: string | null;
  bio: string | null;
  email: string;
  phone: string | null;
  location: string | null;
  photoUrl: string | null;
  companyName: string;
  companyIndustry: string | null;
  companyWebsite: string | null;
  jobsPostedCount: number;
  totalApplicants: number;
  activeJobsCount: number;
  role: 'Owner' | 'Admin' | 'Recruiter' | 'Employer';
  permissions: string[];
}
export interface UpdateProfileForm {
  fullName?: string;
  phoneNumber?: string |null;
  location?: string| null;
  position?: string| null;
  bio?: string| null;
}
export interface EmployerJob {
  id: number;
  title: string;
  location?: string;
  jobType: string;
  experienceLevel: string;
  createdAt: string;
  isActive: boolean;
  applicantsCount: number;
  postedBy?: string;
}

export interface HiringPipelineDto {
  new: number;
  applied: number;
  underReview: number;
  shortlisted: number;
  interviewScheduled: number;
  interviewed: number;
  offered: number;
  hired: number;
  rejected: number;
  withdrawn: number;
}

export interface PipelineRow {
  label: string;
  count: number;
  percent: number;
  cls: string;
}
export interface RecentApplicant {
  id: number;
  applicantName: string;
  photoUrl?: string;
  jobId: number;
  jobTitle: string;
  status: string;
  appliedAt: string;
}