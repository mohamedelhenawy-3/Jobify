import { Skill } from "../Skill/skill";

export interface JobSeekerProfile {
  phoneNumber: string;
  resumeUrl: string | null;
  location: string | null;
  profileImage: string | null;
  headline: string | null;
  summary: string | null;
  skills: Skill[];
  applications: any[];
}