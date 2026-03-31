export interface CompanyOwner {
  id: number;
  companyName: string;
  companyWebsite?: string;
  description?: string;
  industry?: string;
  location?: string;
  companySize?: string;
  founded?: string;
  logoUrl?: string;
  createdAt: string; // ISO date from API

  // ===== PRIVATE STATS =====
  totalJobsPosted: number;
  activeJobs: number;
  draftJobs: number;
  closedJobs: number;
  totalApplicationsReceived: number;
  pendingApplications: number;
  acceptedApplications: number;
  rejectedApplications: number;
  interviewScheduled: number;
}