export interface ProfileDto {
  fullName: string;
  email: string;
  photoUrl?: string | null;
  headline?: string | null;
  about?: string | null;
  location?: string | null;
  phone?: string | null;
  linkedInUrl?: string | null;
  gitHubUrl?: string | null;
  websiteUrl?: string | null;
  isOpenToWork: boolean;
  desiredRole?: string | null;
  preferredJobType?: string | null;
  expectedSalary?: number | null;
  noticePeriod?: string | null;
  preferredLocations?: string | null;
  isPublic: boolean;
  username?: string | null;
  showEmail: boolean;
  showPhone: boolean;
  showCv: boolean;
  experiences: any[];
  educations: any[];
  skills: string[];
  strength: number;
  missing: string[];
}
export interface ExperienceDto {
  id?: number;
  title: string;
  company: string;
  location?: string | null;
  startDate: string;          // "yyyy-MM-dd" from the date input
  endDate?: string | null;
  isCurrent: boolean;
  description?: string | null;
}

export interface EducationDto {
  id?: number;
  school: string;
  degree: string;
  fieldOfStudy?: string | null;
  startDate: string;
  endDate?: string | null;
  description?: string | null;
}