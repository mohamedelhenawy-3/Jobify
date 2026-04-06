import { BaseApplication } from "./BaseApplicationDTO";

export interface AppliedJobSeekerDto extends BaseApplication {
  jobTitle: string;
  jobSeekerFullName: string;
  JobSeekerEmail:string;
  profileImageUrl: string;
  resumeUrl: string;
}