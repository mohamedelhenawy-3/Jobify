


import { ApplicationStatus } from "./ApplicationStatus";

export interface JobSeekerApplication
  {
    applicationId: number;
  status: ApplicationStatus;
  appliedAt: string; // ISO date string
  jobId: number;
  jobTitle: string;
  companyName: string;
  jobLocation: string;
  workMode: string;
  employmentType: string;
  coverLetterUrl: string;
}
