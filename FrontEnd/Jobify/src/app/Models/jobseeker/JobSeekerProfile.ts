import { Education } from "../Education/Education";
import { Experience } from "../Experince/Experience";
import { Skill } from "../Skill/skill";

export interface JobSeekerProfile {
   isPrivate: boolean;
  message?: string;
  fullName: string;
  phoneNumber: string;

  location: string | null;
  profileImage: string | null;

  headline: string | null;
  summary: string | null;

  linkedInUrl: string | null;
  githubUrl: string | null;
resumeUrl:string|null;
  skills: Skill[];

  experiences: Experience[];

  educations: Education[];
}