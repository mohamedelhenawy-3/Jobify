import { ApplicationStatus } from "./ApplicationStatus";

export interface UpdateStatus {
  status: ApplicationStatus;
  interviewDate?: string | null;     // Use string for ISO format or null
  interviewLocation?: string | null;
  notes?: string | null;
}