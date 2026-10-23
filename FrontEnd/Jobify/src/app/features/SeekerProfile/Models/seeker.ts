export interface ProfileDto {
  id: number;
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
  languages: LanguageDto[];
  certificates: CertificateDto[];
  projects: ProjectDto[];
  educations: any[];
  skills: string[];
  strength: number;
  missing: string[];
}

export interface LanguageDto {
  id?: number;
  name: string;
  level: string;
}

export interface CertificateDto {
  id?: number;
  name: string;
  issuer: string;
  issueDate: string;
  expiryDate: string | null;
  credentialUrl: string | null;
}

export interface ProjectImageDto {
  id: number;
  imageUrl: string;
  isCover: boolean;
  displayOrder: number;
}

export interface ProjectDto {
  id?: number;
  title: string;
  description: string | null;
  url: string | null;
  technologies: string | null;

  // returned by the API (read only)
  displayOrder?: number;
  coverImageUrl?: string | null;
  images: ProjectImageDto[];
}

export interface ExperienceDto {
  id?: number;
  title: string;
  company: string;
  location?: string | null;
  startDate: string;
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