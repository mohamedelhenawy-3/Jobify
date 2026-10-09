
export enum EmploymentType {
  FullTime = 'FullTime',
  PartTime = 'PartTime',
  Contract = 'Contract',
  Remote = 'Remote',
  Hybrid = 'Hybrid',
  Internship = 'Internship'
}
export type JobStatus = 'Active' | 'Expired' | 'Closed';

export enum ExperianceLevel {
  EntryLevel = 'EntryLevel',
  MidLevel = 'MidLevel',
  Senior = 'Senior',
  Lead = 'Lead',
  Executive = 'Executive'
}



export const EMPLOYMENT_TYPE_LABELS: Record<string, string> = {
  FullTime: 'Full-time',
  PartTime: 'Part-time',
  Contract: 'Contract',
  Remote: 'Remote',
  Hybrid: 'Hybrid',
  Internship: 'Internship'
};

export const EXPERIANCE_LEVEL_LABELS: Record<string, string> = {
  EntryLevel: 'Entry level',
  MidLevel: 'Mid level',
  Senior: 'Senior',
  Lead: 'Lead',
  Executive: 'Executive'
};

export const JOB_STATUSES: JobStatus[] = ['Active', 'Expired', 'Closed'];
