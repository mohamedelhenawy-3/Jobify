export type SearchType = 'all' | 'job' | 'user' | 'company';

export enum SearchResultType {
  Job = 'Job',
  User = 'User',
  Company = 'Company'
}

export interface GlobalSearchParams {
  keyword?: string;
  type?: SearchType;
  employmentType?: string;
  experienceLevel?: string;
  postedWithinDays?: number;
  industry?: string;
  companySize?: string;
  location?: string;
}

export interface JobSearchResult {
  id: number;
  globalId: string;
  resultType: SearchResultType;
  logo?: string;
  title?: string;
  description?: string;
  location?: string;
  minSalary?: number;
  maxSalary?: number;
  jobType?: string;
  experienceLevel?: string;
}

/** Mirrors Application.DTOs.JobSeeker.UserSeachDto */
export interface UserSearchResult {
  id: string;
  globalId: string;
  resultType: SearchResultType;
  fullName?: string;
  location?: string;
  profileImage?: string;
  headline?: string;
  summary?: string;
  profileImageUrl?: string;
  userRole: string;
}

/** Mirrors Application.DTOs.Comapny.SearchCompanyDto */
export interface CompanySearchResult {
  id: number;
  globalId: string;
  resultType: SearchResultType;
  companyName: string;
  description?: string;
  location?: string;
  companySize?: string;
  logo?: string;
}

export interface GlobalSearchResponse {
  jobs: JobSearchResult[];
  users: UserSearchResult[];
  companies: CompanySearchResult[];
}

export const EMPTY_SEARCH_RESPONSE: GlobalSearchResponse = {
  jobs: [],
  users: [],
  companies: []
};