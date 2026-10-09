import { ExperienceLevel, JobType } from "../components/update-job/update-job.component";
import { EmploymentType, ExperianceLevel, JobStatus } from "./job-enums";

export interface JobSearchFilter {
  keyword?: string;
  location?: string;
  employmentType?: string;
  experianceLevel?: string;
  minSalary?: number;
  maxSalary?: number;
  sortBy?: 'newest' | 'salary_desc' | 'salary_asc';
  pageNumber: number;
  pageSize: number;
}

export const DEFAULT_JOB_FILTER: JobSearchFilter = {
  pageNumber: 1,
  pageSize: 10,
  sortBy: 'newest'
};



export interface EmployerJobsFilter {
  keyword: string | null;
  status: JobStatus | null;
  employmentType: EmploymentType | null;      // was JobType
  experienceLevel: ExperianceLevel | null;    // was ExperienceLevel
  sortBy: 'newest' | 'oldest' | 'applicants_desc';
  pageNumber: number;
  pageSize: number;
}
export const DEFAULT_EMPLOYER_JOBS_FILTER: EmployerJobsFilter = {
  keyword: null,
  status: null,
  employmentType: null,
  experienceLevel: null,
  sortBy: 'newest',
  pageNumber: 1,
  pageSize: 10
};