import { EmploymentType ,ExperianceLevel, JobStatus} from "./job-enums";
import { PostedWithin } from "./job-search-filter.model";

export interface Job {
  id: number;
  logo?: string | null;
  title: string;
  description: string;
  location: string;
  minSalary?: number | null;
  maxSalary?: number | null;
  employmentType: string;
  exprianceLevel: string;
  status: string;

  createdAt: string;
  postedAge: string;

  companyName: string;
  companyAge: string;

  totalApplications: number;
  postedByEmployerId?: string | null;
  postedByName?: string | null;
  isOwnerView: boolean;
}

export interface JobCard {
  id: number;
  title: string;
  logo?: string | null;
  companyName?: string;
  location: string;
  employmentType: EmploymentType;
  experienceLevel: ExperianceLevel;
  minSalary?: number | null;
  maxSalary?: number | null;
  currency: string;
  createdAt: string;
  expirationDate: string;
  postedAge: string;
  // employer-only, optional
  status?: JobStatus;
  totalApplications?: number;
  postedByName?: string;
  isOwnerView?: boolean;
}

export interface CreateJobRequest {
  title: string;
  description: string;
  location: string;
  minSalary: number;
  maxSalary: number;
  currency: string;
  requirements: string;
  responsibilities: string;
  jobType: EmploymentType;
  experienceLevel: ExperianceLevel;
  isActive: boolean;
  expirationDate: string | null; // ISO string, e.g. 2026-09-29T19:58:02.505Z
}
export interface JobDetails {
  id: number;
  title: string;
  description: string;
  location: string;
  minSalary: number;
  maxSalary: number;
  employmentType: string;
  experienceLevel: string;
  requirements: string;
  responsibilities: string;
  createdAt: string;
  postedAge: string;
  companyId: number;
  companyName: string;
  companyLogo: string | null;
  companyWebsite: string | null;
  hasApplied: boolean;
  applicationId: number | null;
  applicationStatus: string | null;
  isActive: boolean;
}

export type EmployerJobCard = JobCard;
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}
export interface JobMatch {
  id: number;
  title: string;
  companyName?: string;
  location?: string;
  jobType?: string;
  experienceLevel?: string;
  minSalary?: number;
  maxSalary?: number;
  logo?: string;
  createdAt: string;
  matchPercent: number;
}

export interface JobAlert {
   id: number;
   keyword: string;
   isActive: boolean;
   createdAt: string; 
  }



  export interface RecommendedJob {
  jobId: number;
  title: string;
  logoUrl: string | null;
  companyName: string | null;
  location: string | null;
  employmentType: string;
  salaryMin: number | null;
  salaryMax: number | null;
  currency: string | null;
  matchScore: number;
  matchedSkills: string[];
  postedAtUtc: string;
}


export interface LatestJob {
  id: number;
  title: string;
  companyName: string;
  companyLogoUrl: string | null;
  location: string;
  jobType: string;
  salaryMin: number | null;
  salaryMax: number | null;
  createdAt: string;
}


