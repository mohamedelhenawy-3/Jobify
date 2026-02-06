import { Skill } from "../Skill/skill";

export interface JobSeekerProfile {
  fullName:string;
  phoneNumber: string;
  resume: string | null;
  location: string | null;
  profileImage: string | null;
  headline: string | null;
  summary: string | null;
  skills: Skill[];
  applications: any[];
}