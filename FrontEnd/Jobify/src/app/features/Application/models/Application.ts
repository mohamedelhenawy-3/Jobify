export enum ApplicationStatus {
  Applied = 1,
  UnderReview = 2,
  Shortlisted = 3,
  InterviewScheduled = 4,
  Interviewed = 5,
  Offered = 6,
  Hired = 7,
  Rejected = 8,
  Withdrawn = 9
}
export interface JobApplicationListItem {
  id: number;
  status: ApplicationStatus;
  appliedAt: string;
  jobTitle: string;
  logo: string;
  fullName?: string;   
  email?: string;     
}

export interface JobApplicationFilter {
  status?: ApplicationStatus | null;
  searchTerm?: string;
}


export interface UpdateAppliedJobStatus {
  status: ApplicationStatus;
  interviewDate?: string | null;
  interviewLocation?: string | null;
  notes?: string | null;
}

export interface InterviewInfo {
  interviewDate: string;
  location: string;
  notes?: string | null;
}

export interface ApplicationDetails {
  id: number;
  status: ApplicationStatus;
  appliedAt: string;
  jobPostId: number;
  jobTitle: string;
  fullName: string;
  email: string;
  phone?: string | null;
  logo?: string | null;
  coverLetter?: string | null;
  resumeUrl?: string | null;
  interview?: InterviewInfo | null;
}

