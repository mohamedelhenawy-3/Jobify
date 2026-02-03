import { ApplicationStatus } from "./ApplicationStatus";

export interface AppliedJobSeekerListItem  {
  applicationId: number;
  profileImageUrl: string;
  jobTitle: string;
  joSeekerEmail: string;
  joSeekerFullName: string;
  status: ApplicationStatus;
  appliedAt: string; // ISO date string from backend
}
