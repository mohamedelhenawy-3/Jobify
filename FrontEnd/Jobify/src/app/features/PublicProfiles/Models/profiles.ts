export interface PublicJobSummary {
  id: number;
  title: string;
  location?: string | null;
  createdAt?: string | null;
}

interface PublicUserBase {
  userId: string;
  fullName: string;
  imageUrl?: string | null;
  isPrivate: boolean;
}
export interface PublicEmployerProfile extends PublicUserBase {
  type: 'employer';
  employerId: number;
  bio?: string | null;
  position?: string | null;
  linkedInUrl?: string | null;
  role?: string | null;
  joinedCompanyAt?: string | null;
  jobsPosted: number;
  openJobsCount: number;
  recentJobs: PublicJobSummary[];
  companyId?: number | null;
  companyName?: string | null;
  companyLogo?: string | null;
  
}

export interface PublicExperience {
  title: string;
  company: string;
  location?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  isCurrent: boolean;
  description?: string | null;
}

export interface PublicEducation {
  school: string;
  degree?: string | null;
  fieldOfStudy?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  description?: string | null;
}

export interface PublicJobSeekerProfile extends PublicUserBase {
  type: 'jobseeker';
  seekerId: number;
  headline?: string | null;
  location?: string | null;
  about?: string | null;
  summary?: string | null;
  githubUrl?: string | null;
  websiteUrl?: string | null;
  isOpenToWork: boolean;
  experiences: PublicExperience[];
  educations: PublicEducation[];
  skills: string[];
}

export type PublicProfile = PublicEmployerProfile | PublicJobSeekerProfile;


export interface PublicJobSummary {
  id: number;
  title: string;
  location?: string | null;
  createdAt?: string | null;
}

export interface PublicCompanyMember {
  userId: string;               // Users.Id, opens /profile/{userId}
  fullName: string;
  imageUrl?: string | null;
  position?: string | null;
  role?: string | null;
}

export interface PublicCompany {
  type: string;
  id: number;
  companyName: string;
  logo?: string | null;
  industry?: string | null;
  location?: string | null;
  description?: string | null;
  companySize?: string | null;
  founded?: number | null;
  companyWebsite?: string | null;
  isVerified: boolean;
  membersCount: number;
  openJobsCount: number;
  openJobs: PublicJobSummary[];
}