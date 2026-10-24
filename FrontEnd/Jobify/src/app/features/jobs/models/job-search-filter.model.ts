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
export type PostedWithin = '' | 'day' | '3days' | 'week' | 'month';


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
    industry: null,
  postedWithin: null,
  pageNumber: number;
  pageSize: number;
}
export const DEFAULT_EMPLOYER_JOBS_FILTER: EmployerJobsFilter = {
  keyword: null,
  status: null,
  employmentType: null,
  experienceLevel: null,
  sortBy: 'newest',
    industry: null,
  postedWithin: null,
  pageNumber: 1,
  pageSize: 10
};

export const POSTED_WITHIN_OPTIONS: { value: PostedWithin; label: string }[] = [
  { value: 'day',   label: 'Last 24 hours' },
  { value: '3days', label: 'Last 3 days' },
  { value: 'week',  label: 'Last week' },
  { value: 'month', label: 'Last month' },
]